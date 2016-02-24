/*-------------------------------------------------------------------*/
/*  Copyright(C) 2012 by OMRON Corporation                           */
/*  All Rights Reserved.                                             */
/*                                                                   */
/*   This source code is the Confidential and Proprietary Property   */
/*   of OMRON Corporation.  Any unauthorized use, reproduction or    */
/*   transfer of this software is strictly prohibited.               */
/*                                                                   */
/*-------------------------------------------------------------------*/

#ifndef OKAOIMAGE_H__
#define OKAOIMAGE_H__

#include "OkaoTypedef.h"

/* Image Format Codes */
#define     YUV422_Y0CbY1Cr             0x01404813
#define     YUV422_Y1CbY0Cr             0x01406813
#define     YUV422_Y0CrY1Cb             0x01404831
#define     YUV422_Y1CrY0Cb             0x01406831
#define     YUV422_CbY0CrY1             0x01414802
#define     YUV422_CrY0CbY1             0x01414820
#define     YUV422_CbY1CrY0             0x01416802
#define     YUV422_CrY1CbY0             0x01416820
#define     YUV420SP_Y0Y1Y2Y3_CbCr      0x06500545
#define     YUV420SP_Y3Y2Y1Y0_CbCr      0x06D03545
#define     YUV420SP_Y0Y1Y2Y3_CrCb      0x06500554
#define     YUV420SP_Y3Y2Y1Y0_CrCb      0x06D03554
#define     YUV420FP_Y0Y1Y2Y3_Cb_Cr     0x0B500148
#define     YUV420FP_Y3Y2Y1Y0_Cb_Cr     0x0BD03148
#define     GRAY_Y0Y1Y2Y3               0x21000044
#define     GRAY_Y3Y2Y1Y0               0x21C03044
#define     RGBA8888                    0x41088888
#define     RGB888                      0x45068888
#define     RGB565                      0x49050565
#define     RGB555                      0x4D051555

enum eIMG_PLANAR { 
    eIMG_PLANAR_FIRST = 0,
    eIMG_PLANAR_SECOND,
    eIMG_PLANAR_THIRD,
    eIMG_PLANAR_MAX
};

typedef enum tagYUV420_FORMAT {
    YUV420_FORMAT_CBCR = 0,
    YUV420_FORMAT_CRCB,
    YUV420_FORMAT_MAX
} YUV420_FORMAT;

/* Image Information */
typedef struct tagIMGINFO {
    UINT32      unImageFormat;              /* Image Format */
    INT32       nWidth;                     /* Image Width */
    INT32       nHeight;                    /* Image Height */
    INT32       nReserved;                  
    RAWIMAGE    *apImage[eIMG_PLANAR_MAX];  /* Image Planars */
} IMGINFO;

#endif /* OKAOIMAGE_H__ */
