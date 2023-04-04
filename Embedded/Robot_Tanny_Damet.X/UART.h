/* 
 * File:   UART.h
 * Author: TABLE 6
 *
 * Created on 4 avril 2023, 09:32
 */

#ifndef UART_H
#define UART_H

void InitUART(void);

void SendMessageDirect(unsigned char* message, int length);

void __attribute__((interrupt, no_auto_psv)) _U1RXInterrupt(void);


#endif /* UART_H */


