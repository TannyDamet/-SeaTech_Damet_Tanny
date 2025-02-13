/* 
 * File:   Robot.h
 * Author: TP-EO-1
 *
 * Created on 7 d�cembre 2022, 15:07
 */

#ifndef ROBOT_H
#define ROBOT_H
typedef struct robotStateBITS {
union {

struct {
unsigned char taskEnCours;
float vitesseGaucheConsigne;
float vitesseGaucheCommandeCourante;
float vitesseDroiteConsigne;
float vitesseDroiteCommandeCourante;
float distanceTelemetreDroit_1;
float distanceTelemetreDroit_2;
float distanceTelemetreCentre;
float distanceTelemetreGauche_1;
float distanceTelemetreGauche_2;


}
;}
;} ROBOT_STATE_BITS;

extern volatile ROBOT_STATE_BITS robotState;
#endif /* ROBOT_H */
