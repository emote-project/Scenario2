/*-------------------------------------------------------------------*/
/*  Copyright(C) 2013 by OMRON Corporation                           */
/*  All Rights Reserved.                                             */
/*                                                                   */
/*   This source code is the Confidential and Proprietary Property   */
/*   of OMRON Corporation.  Any unauthorized use, reproduction or    */
/*   transfer of this software is strictly prohibited.               */
/*                                                                   */
/*-------------------------------------------------------------------*/
/*
    Expression Estimation Library Ver.1
*/

#ifndef OKAOEXAPI_H__
#define OKAOEXAPI_H__

#define OKAO_API
#include "OkaoDef.h"

#ifndef OKAO_DEF_HCOMMON
#define OKAO_DEF_HCOMMON
    typedef VOID*   HCOMMON;
#endif /* OKAO_DEF_HCOMMON */

#ifndef OKAO_DEF_HEXPRESSION
#define OKAO_DEF_HEXPRESSION
    typedef void *HEXPRESSION;       /* Expression Estimation handle */
#endif /* OKAO_DEF_HEXPRESSION */

#ifndef OKAO_DEF_HPTRESULT
#define OKAO_DEF_HPTRESULT
    /* Facial Parts Detection result handle */
    typedef void *HPTRESULT;
#endif /* OKAO_DEF_HPTRESULT */

/* Estiomation Result */
typedef enum {
    EX_EXPRESSION_NEUTRAL = 0, /* Neutral */
    EX_EXPRESSION_HAPPINESS,   /* Happiness */
    EX_EXPRESSION_SURPRISE,    /* Surpprise*/
    EX_EXPRESSION_FEAR,        /* Fear */
    EX_EXPRESSION_ANGER,       /* Anger */
    EX_EXPRESSION_DISGUST,     /* Disgust */
    EX_EXPRESSION_SADNESS,     /* Sadness */
    EX_EXPRESSION_KIND_MAX     /* The number of expressions */
}EX_EXPRESSION;


typedef struct {
    INT32     anScore[EX_EXPRESSION_KIND_MAX]; /* Score for each facial expression category */
    INT32     nExpressionDegree;               /* Negative - Positive  */
//    INT32     nReserved[]; 
}EX_RESULT;

#ifdef  __cplusplus
extern "C" {
#endif

/**********************************************************/
/* Version infomation                                     */
/**********************************************************/
/* Get Expression Estimation Library API Version */
OKAO_API INT32        OKAO_EX_GetVersion(UINT8 *pucMajor, UINT8 *pucMinor);


/**********************************************************/
/* Handle Creation/Deletion                               */
/**********************************************************/
/* Creates/Deletes Property Estimation Handle */
OKAO_API HEXPRESSION  OKAO_EX_CreateHandle(HCOMMON hCO);
OKAO_API INT32        OKAO_EX_DeleteHandle(HEXPRESSION hEX);


/**********************************************************/
/* Setting of Facial Parts Position                       */
/**********************************************************/
/* Set the facial parts  position */
OKAO_API INT32        OKAO_EX_SetPoint(HEXPRESSION hEX, POINTINFO stPointInfo);
/* Sets facial parts position from PT result handle */
OKAO_API INT32        OKAO_EX_SetPointFromHandle(HEXPRESSION hEX, HPTRESULT hPtResult);


/**********************************************************/
/* Excecution of Expression Estimation                    */
/**********************************************************/
/* Estimate the expression score */
OKAO_API INT32 OKAO_EX_Estimate(HEXPRESSION hEX, IMGINFO stImgInfo, EX_RESULT *pstResult);


#ifdef  __cplusplus
}
#endif


#endif  /* OKAOEXAPI_H__ */
