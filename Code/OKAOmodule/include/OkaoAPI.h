/*-------------------------------------------------------------------*/
/*  Copyright(C) 2003-2012 by OMRON Corporation                      */
/*  All Rights Reserved.                                             */
/*                                                                   */
/*   This source code is the Confidential and Proprietary Property   */
/*   of OMRON Corporation.  Any unauthorized use, reproduction or    */
/*   transfer of this software is strictly prohibited.               */
/*                                                                   */
/*-------------------------------------------------------------------*/
/* 
    OKAO_SDK Library API
*/
#ifndef OKAOCOMAPI_H__
#define OKAOCOMAPI_H__

#define OKAO_API
#include "OkaoDef.h"
#include "DetectionInfo.h"

#ifndef OKAO_DEF_HCOMMON
#define OKAO_DEF_HCOMMON
    typedef VOID*   HCOMMON;
#endif /* OKAO_DEF_HCOMMON */

/* Square Corners Indices */
enum eSQUARE_POSITION { 
    eSQUARE_POSITION_LEFTTOP = 0,
    eSQUARE_POSITION_RIGHTTOP,
    eSQUARE_POSITION_LEFTBOTTOM,
    eSQUARE_POSITION_RIGHTBOTTOM,
    eSQUARE_POSITION_MAX_NUM
};

/* Memory Information */
typedef struct tagMEMINFO {
    VOID    *pArea;     /* Address of Memory Area */
    UINT32  unSize;     /* Size of Memory Area */
    INT32   nReserved;
} MEMINFO;

/* Square in Center-Form */
typedef struct tagPOSITIONCENTER {
    POINT   ptCenter;   /* Center Point */
    INT32   nSize;      /* Size */
    INT32   nAngle;     /* Angle */
} POSITIONCENTER;

typedef VOID    *(*ALLOCATOR)( SIZE_T );
typedef VOID    (*DEALLOCATOR)( VOID* );

typedef POINT   POSITIONSQUARE;


#ifdef  __cplusplus
extern "C" {
#endif

/************************************************************/
/* Get Version                                              */
/************************************************************/
/* Get Version */
OKAO_API INT32      OKAO_CO_GetVersion(UINT8 *pucMajor, UINT8 *pucMinor);

/************************************************************/
/* Common Function Handle Creation/Deletion                 */
/************************************************************/
/* Creation */
OKAO_API HCOMMON    OKAO_CO_CreateHandle(MEMINFO *pstBMemInfo, MEMINFO *pstWMemInfo, 
                                        ALLOCATOR AllocateFunction, DEALLOCATOR DeallocateFunction);

/* Deletion */
OKAO_API INT32      OKAO_CO_DeleteHandle(HCOMMON hCO);

/************************************************************/
/* ÉÅÉÇÉää«óù                                               */
/************************************************************/
/* ÉèÅ[ÉNÉÅÉÇÉäÇÃê›íË */
OKAO_API INT32      OKAO_CO_ReplaceWMemInfo(HCOMMON hCO, MEMINFO *pstWMemInfo);

/************************************************************/
/* Image Conversion                                         */
/************************************************************/
/* Image Conversion */
OKAO_API INT32      OKAO_CO_ConvertImage(IMGINFO srcImgInfo, IMGINFO dstImgInfo);

/************************************************************/
/* Square Points to Center-Form                             */
/************************************************************/
/* Conversion from Square Points to Center-Form */
OKAO_API INT32      OKAO_CO_ConvertSquareToCenter(POSITIONSQUARE aptSquare[eSQUARE_POSITION_MAX_NUM],
                                                                                POSITIONCENTER *pstCenter);

/************************************************************/
/* Center-Form to Square Points                             */
/************************************************************/
/* Convertsion from Center-Form to Square Points */
OKAO_API INT32      OKAO_CO_ConvertCenterToSquare(POSITIONCENTER stCenter,
                                                        POSITIONSQUARE aptSquare[eSQUARE_POSITION_MAX_NUM]);

#ifdef  __cplusplus
}
#endif

#endif  /* OKAOCOMAPI_H__ */
