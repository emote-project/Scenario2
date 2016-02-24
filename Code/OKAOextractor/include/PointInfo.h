/*-------------------------------------------------------------------*/
/*  Copyright(C) 2012 by OMRON Corporation                           */
/*  All Rights Reserved.                                             */
/*                                                                   */
/*   This source code is the Confidential and Proprietary Property   */
/*   of OMRON Corporation.  Any unauthorized use, reproduction or    */
/*   transfer of this software is strictly prohibited.               */
/*                                                                   */
/*-------------------------------------------------------------------*/

#ifndef POINTINFO_H__
#define POINTINFO_H__

#include "OkaoTypedef.h"

/* Feature Points from Facial Parts Detection */
enum ePOINT { 
    ePOINT_LEFT_EYE = 0,            /* Left Eye Center    */
    ePOINT_RIGHT_EYE,               /* Right Eye Center   */
    ePOINT_MOUTH,                   /* Mouth Center       */
    ePOINT_LEFT_EYE_IN,             /* Left Eye In        */
    ePOINT_LEFT_EYE_OUT,            /* Left Eye Out       */
    ePOINT_RIGHT_EYE_IN,            /* Right Eye In       */
    ePOINT_RIGHT_EYE_OUT,           /* Right Eye Out      */
    ePOINT_MOUTH_LEFT,              /* Mouth Left         */
    ePOINT_MOUTH_RIGHT,             /* Mouth Right        */
    ePOINT_NOSE_LEFT,               /* Nose Left          */
    ePOINT_NOSE_RIGHT,              /* Nose Right         */
    ePOINT_MOUTH_UP,                /* Mouth Up           */
    ePOINT_KIND_MAX                 /* The number of Feature Points */
};

/* Facial Parts Position Result */
typedef struct tagPOINTINFO{
    POINT   aptPoint[ePOINT_KIND_MAX];      /* Point Array of Facial Parts Detection Result */
    INT32   anConfidence[ePOINT_KIND_MAX];  /* Confidence of Each Facial Part Point */
    INT32   nUpDown;                        /* Face Direction (Up/Down) */
    INT32   nLeftRight;                     /* Face Direction (Left/Right) */
    INT32   nRoll;                          /* Face Angle */
    INT32   nReserved;
}POINTINFO;

#endif /* POINTINFO_H__ */
