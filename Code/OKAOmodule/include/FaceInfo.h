/*-------------------------------------------------------------------*/
/*  Copyright(C) 2012 by OMRON Corporation                           */
/*  All Rights Reserved.                                             */
/*                                                                   */
/*   This source code is the Confidential and Proprietary Property   */
/*   of OMRON Corporation.  Any unauthorized use, reproduction or    */
/*   transfer of this software is strictly prohibited.               */
/*                                                                   */
/*-------------------------------------------------------------------*/

#ifndef FACEINFO_H__
#define FACEINFO_H__

#include "OkaoTypedef.h"

/* Face Detection Information */
typedef struct tagFACEINFO {
    INT32           nID;            /* ID Number */
    POINT           ptLeftTop;      /* Left-Top     Face Coordinates    */
    POINT           ptRightTop;     /* Right-Top    Face Coordinates    */
    POINT           ptLeftBottom;   /* Left-Bottom  Face Coordinates    */
    POINT           ptRightBottom;  /* Right-Bottom Face Coordinates    */
    INT32           nPose;          /* Face Pose                        */
    INT32           nConfidence;    /* Confidence Degree                */
} FACEINFO;

#endif /* FACEINFO_H__ */
