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

#endif /* UART_H */


