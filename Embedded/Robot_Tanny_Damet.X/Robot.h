/* 
 * File:   Robot.h
 * Author: TP-EO-1
 *
 * Created on 7 décembre 2022, 15:07
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
float distanceTelemetreDroit;
float distanceTelemetreCentre;
float distanceTelemetreGauche;


}
;}
;} ROBOT_STATE_BITS;

extern volatile ROBOT_STATE_BITS robotState;
#endif /* ROBOT_H */
