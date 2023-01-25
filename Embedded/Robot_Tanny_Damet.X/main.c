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
/*
 * 
 */


    
int main (void){
/***************************************************************************************************/

//Initialisation de l?oscillateur
/****************************************************************************************************/

InitOscillator();

/****************************************************************************************************/

// Configuration des entrées sorties

/****************************************************************************************************/

InitIO();
InitTimer23();
InitTimer1();
InitPWM();
InitADC1();
PWMSetSpeedConsigne(-25, MOTEUR_DROIT);
PWMSetSpeedConsigne(-25, MOTEUR_GAUCHE);
//InitQEI1();
//InitQEI2();


LED_BLANCHE = 1;
LED_BLEUE = 1;
LED_ORANGE = 1;

/****************************************************************************************************/

// Boucle Principale

/****************************************************************************************************/

while(1){
   // LED_BLANCHE = !LED_BLANCHE;
    // LED_BLEUE = !LED_BLEUE;

    //PWMSetSpeedConsigne(-20,MOTEUR_GAUCHE);
    unsigned int * result = ADCGetResult();
    ADC1StartConversionSequence();
    
    unsigned int * ADC1BUF0 = result[0];
    unsigned int * ADC1BUF1 = result[1];
    unsigned int * ADC1BUF2 = result[2];
    
    
    
} 

ADCIsConversionFinished();

ADCClearConversionFinishedFlag();


// fin main
}
    
    
    
    
    
    
    
    
    
    
    
    



