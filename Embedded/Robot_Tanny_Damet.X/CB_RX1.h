/* 
 * File:   CB_RX1.h
 * Author: TABLE 6
 *
 * Created on 7 avril 2023, 13:45
 */

#ifndef CB_RX1_H
#define	CB_RX1_H

#ifdef	__cplusplus
extern "C" {
#endif

    void CB_RX1_Add(unsigned char value);

    unsigned char CB_RX1_Get(void);

    unsigned char CB_RX1_IsDataAvailable(void);

    void __attribute__((interrupt, no_auto_psv)) _U1RXInterrupt(void);

    int CB_RX1_GetDataSize(void);

    int CB_RX1_GetRemainingSize(void);


#ifdef	__cplusplus
}
#endif

#endif	/* CB_RX1_H */




