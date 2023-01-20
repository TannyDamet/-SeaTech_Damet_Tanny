/* 
 * File:   PWM.h
 * Author: TP-EO-1
 *
 * Created on 7 décembre 2022, 15:44
 */

#ifndef PWM_H
#define	PWM_H

#define MOTEUR_GAUCHE 1
#define MOTEUR_DROIT 0

void InitPWM(void);

//void PWMUpdateSpeed(void);
void PWMSetSpeed(float vitesseEnPourcents, unsigned char moteur);

void PWMUpdateSpeed();

void PWMSetSpeedConsigne(float vitesseEnPourcents, char moteur);
#endif	/* PWM_H */

