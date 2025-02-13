/* 
 * File:   main.c
 * Author: TP-EO-1
 *
 * Created on 16 novembre 2022, 10:48
 */

#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "ADC.h"
#include "Robot.h" 
#include "main.h"
#include "UART.h"
#include "CB_TX1.h"
#include "CB_RX1.h"
#include <libpic30.h>
/*
 * 
 */



int main(void) {
    /***************************************************************************************************/

    //Initialisation de l?oscillateur
    /****************************************************************************************************/

    InitOscillator();

    /****************************************************************************************************/

    // Configuration des entr�es sorties

    /****************************************************************************************************/

    InitIO();
    InitTimer23();
    InitTimer1();
    InitTimer4();
    InitPWM();
    InitADC1();
    void InitUART();

    
    //    PWMSetSpeedConsigne(-25, MOTEUR_DROIT);
    //    PWMSetSpeedConsigne(-25, MOTEUR_GAUCHE);


    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;

    /****************************************************************************************************/

    // Boucle Principale

    /****************************************************************************************************/

    while (1) {
        // LED_BLANCHE = !LED_BLANCHE;
        // LED_BLEUE = !LED_BLEUE;

        if (ADCIsConversionFinished() == 1) {

            ADCClearConversionFinishedFlag();

            unsigned int * result = ADCGetResult();

            float volts = ((float) result [1])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit_1 = 34 / volts - 5;
            volts = ((float) result [2])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;
            volts = ((float) result [4])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche_1 = 34 / volts - 5;
            volts = ((float) result [0])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit_2 = 34 / volts - 5;
            volts = ((float) result [3])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche_2 = 34 / volts - 5;

            
            UartEncodeAndSendMessage(0x0020, sizeof(payload), payload);

            
//        unsigned char payload[] = {'B', 'o', 'n', 'j', 'o', 'u', 'r'}; 
        
//        UartEncodeAndSendMessage(0x0080, sizeof(payload), payload);
//        
//        __delay32(40000000);
 
            

            if (robotState.distanceTelemetreDroit_2 > 30) {
                LED_ORANGE = 1;
            } else LED_ORANGE = 0;
            
            if (robotState.distanceTelemetreDroit_1 > 30) {
                LED_ORANGE = 1;

            } else 
                LED_ORANGE = 0;


            if (robotState.distanceTelemetreCentre > 30) {
                LED_BLEUE = 1;
            } else LED_BLEUE = 0;
                
                
            if (robotState.distanceTelemetreGauche_2 > 30) {
                LED_BLANCHE = 1;
            } else LED_BLANCHE = 0;
                
             if (robotState.distanceTelemetreGauche_1 > 30) {
                LED_BLANCHE = 1;

            } else 
                LED_BLANCHE = 0;
        }

//       SendMessage((unsigned char*) "Bonjour", 7);
//       SendMessageDirect((unsigned char*) "Bonjour", 7);
//        __delay32(40000000);

        int i;
        for (i = 0; i < CB_RX1_GetDataSize(); i++) {
            unsigned char c = CB_RX1_Get();
            SendMessage(&c, 1);
        }
        __delay32(10000);
        
//        

 

    }
    
    
    
}


// fin main


unsigned char stateRobot;

void OperatingSystemLoop(void) {
    switch (stateRobot) {
        case STATE_ATTENTE:
            timestamp = 0;
            PWMSetSpeedConsigne(0, MOTEUR_DROIT);
            PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
            stateRobot = STATE_ATTENTE_EN_COURS;

        case STATE_ATTENTE_EN_COURS:
            if (timestamp > 1000)
                stateRobot = STATE_AVANCE;
            break;

        case STATE_AVANCE:
            PWMSetSpeedConsigne(30, MOTEUR_DROIT);
            PWMSetSpeedConsigne(30, MOTEUR_GAUCHE);
            stateRobot = STATE_AVANCE_EN_COURS;
            break;
        case STATE_AVANCE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_GAUCHE:
            PWMSetSpeedConsigne(30, MOTEUR_DROIT);
            PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_DROITE:
            PWMSetSpeedConsigne(0, MOTEUR_DROIT);
            PWMSetSpeedConsigne(30, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_DROITE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_GAUCHE:
            PWMSetSpeedConsigne(15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(-15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_DROITE:
            PWMSetSpeedConsigne(-15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;
            
        case STATE_RECULE:
            PWMSetSpeedConsigne(-15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(-15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
            break;
        case STATE_RECULE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        default:
            stateRobot = STATE_ATTENTE;
            break;
    }
}

unsigned char nextStateRobot = 0;

void SetNextRobotStateInAutomaticMode() {
    unsigned char positionObstacle = PAS_D_OBSTACLE;

    //D�termination de la position des obstacles en fonction des t�l�m�tres
    if ((robotState.distanceTelemetreDroit_2 < 25 &&
            robotState.distanceTelemetreDroit_1 < 30 &&
            robotState.distanceTelemetreCentre > 20 &&
            robotState.distanceTelemetreGauche_1 > 30) 
            || 
        	(robotState.distanceTelemetreDroit_2 < 30 &&
            robotState.distanceTelemetreCentre > 20 &&
            robotState.distanceTelemetreGauche_1 > 30))
            
               // Obstacle � droite
        positionObstacle = OBSTACLE_A_DROITE;

    else if ((robotState.distanceTelemetreDroit_1 > 30 &&
           robotState.distanceTelemetreCentre > 20 &&
           robotState.distanceTelemetreGauche_1 < 30 &&
           robotState.distanceTelemetreGauche_2 < 25)
           ||
           (robotState.distanceTelemetreDroit_2 > 30 &&
           robotState.distanceTelemetreCentre > 20 &&
           robotState.distanceTelemetreGauche_1 < 30))
             //Obstacle � gauche
        
        positionObstacle = OBSTACLE_A_GAUCHE;


    else if (robotState.distanceTelemetreCentre < 20) //Obstacle en face
        positionObstacle = OBSTACLE_EN_FACE;

    else if (robotState.distanceTelemetreDroit_2 > 30 &&
            robotState.distanceTelemetreCentre > 20 &&
            robotState.distanceTelemetreGauche_2 > 30) //pas d?obstacle
        positionObstacle = PAS_D_OBSTACLE;

    //D�termination de l?�tat � venir du robot
    if (positionObstacle == PAS_D_OBSTACLE)
        nextStateRobot = STATE_AVANCE;
    else if (positionObstacle == OBSTACLE_A_DROITE)
        nextStateRobot = STATE_TOURNE_GAUCHE;
    else if (positionObstacle == OBSTACLE_A_GAUCHE)
        nextStateRobot = STATE_TOURNE_DROITE;
    else if (positionObstacle == OBSTACLE_EN_FACE)
        nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;

    //Si l?on n?est pas dans la transition de l?�tape en cours
    if (nextStateRobot != stateRobot - 1)
        stateRobot = nextStateRobot;



}








