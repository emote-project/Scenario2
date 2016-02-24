#include "opencv2/opencv.hpp"
#include <stdio.h>
#include <string.h>
#include "OkaoAPI.h"
#include "CommonDef.h"
#include "OkaoDtAPI.h"
#include "OkaoPtAPI.h"
#include "iostream"
#include "OkaoExAPI.h"
#include "OkaoSmAPI.h"
#include "fstream"
#include "OkaoGbAPI.h"

HANDLE hPipe1;
cv::Rect getCvRect (const FACEINFO & sFaceInfo)
{
    cv::Point center ((sFaceInfo.ptLeftTop.x + sFaceInfo.ptRightBottom.x) / 2,
                      (sFaceInfo.ptLeftTop.y + sFaceInfo.ptRightBottom.y) / 2);
    cv::Point side (sFaceInfo.ptLeftTop.x - sFaceInfo.ptLeftBottom.x,
                    sFaceInfo.ptLeftTop.y - sFaceInfo.ptLeftBottom.y);
    int width = std::sqrt(side.ddot(side));
    return cv::Rect(center.x - width / 2, center.y - width / 2, width, width);
}

using namespace std;
using namespace cv;
void sendpipe(string messagetxt)
{
	char a[250];
	sprintf(a, "%s", messagetxt.c_str());

	LPTSTR lpszPipename1 = TEXT("\\\\.\\pipe\\serverpipe");
	DWORD cbWritten;
	DWORD dwBytesToWrite = (DWORD)strlen(a);
	hPipe1 = CreateFile(lpszPipename1, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
	if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE))
	{
		printf("Could not open the pipe  - (error %d)\n", GetLastError());
	}
	else
	{
		WriteFile(hPipe1, a, dwBytesToWrite, &cbWritten, NULL);
		memset(a, 0xCC, 100);
		CloseHandle(hPipe1);
	}
}
int main(int argc, char* argv[])
{
	int selectedCamera = 0;
			int scenario = 1;
			if (argc>1) 
			{
				char* p;
				if (strtol(argv[2], &p, 10) == 1)
					scenario = 1;
				else
					scenario = 2;

				selectedCamera = strtol(argv[1], &p, 10);
			}
			
			
			cv::VideoCapture webcam(selectedCamera);
			
			
			INT32 nRet = OKAO_ERR_VARIOUS;  /* Return code */
			INT32 nIndex = 0;                   /* Index of face detection */
			INT32 anConf[PT_POINT_KIND_MAX];
			POINT aptPoint[PT_POINT_KIND_MAX];
			INT32 nGazeLeftRight;           /* Right and left estimation result angle for gaze */
			INT32 nGazeUpDown;              /* Upper and lower estimation result angle for gaze */
			INT32 nCloseRatioLeftEye;       /* left eye estimation result for blink */
			INT32 nCloseRatioRightEye;      /* Right eye estimation result for blink */
			INT32 nSmile;
			INT32 userID;
			INT32 nConfidence;

			INT32 nMaxFaceNumber = 1;
			INT32 nMaxSwapNumber = 0;
			char *pExp[EX_EXPRESSION_KIND_MAX] = { "Neutral:", " Happiness:", " Surprise:", " Fear:", " Anger:", " Disgust:", " Sadness:" };
			IMGINFO stImage;
			EX_RESULT stExResult;
			HCOMMON hCO = NULL;
			HDETECTION hDT = NULL;          /* Face Detection Handle */
			HDTRESULT hDtResult = NULL;     /* Face Detection Result Handle */
			HPOINTER hPT = NULL;            /* Facial Parts Detection Handle */
			HPTRESULT hPtResult = NULL;     /* Facial Parts Detection Result Handle */
			HSMILE hSM = NULL;              /* Smile Degree Estimation Handle */
			HSMRESULT hSmResult = NULL;     /* Smile Degree Estimation Result Handle */
			HEXPRESSION hEX = NULL;         /* Expression Estimation Handle */
			HGAZEBLINK hGB = NULL;          /* Gaze and Blink Estimation Handle */
			HGBRESULT hGbResult = NULL;     /* Gaze and Blink Estimation Result Handle */

			//		INT32 nCloseRatioLeftEye;       /* left eye estimation result for blink */
			//		INT32 nCloseRatioRightEye;      /* Right eye estimation result for blink */
			//		INT32 nSmile;
			INT32 up = 0;
			INT32 down = 0;
			INT32 left = 0;
			INT32 right = 0;
			INT32 fup = 0;
			INT32 fdown = 0;
			INT32 fleft = 0;
			INT32 fright = 0;

			INT32 updown = 0;

			INT32 leftright = 0;
			INT32 Fupdown = 0;
			INT32 Fleftright = 0;
			INT32 nFupdown = 0;
			INT32 nFleftright = 0;
			INT32 Froll = 0;

			int timess = 0;
			INT32 up2 = 0;
			INT32 down2 = 0;
			INT32 left2 = 0;
			INT32 right2 = 0;
			INT32 fup2 = 0;
			INT32 fdown2 = 0;
			INT32 fleft2 = 0;
			INT32 fright2 = 0;

			INT32 updown2 = 0;
			INT32 nGazeLeftRight2 = 0;           /* Right and left estimation result angle for gaze */
			INT32 nGazeUpDown2 = 0;
			INT32 leftright2 = 0;
			INT32 nFupdown2 = 0;
			INT32 nFleftright2 = 0;


			int timess2 = 0;
			std::cout << "To exit the program, press 'q' key" << std::endl;
			
			hCO = OKAO_CO_CreateHandle(NULL, NULL, (ALLOCATOR)malloc, free);
			if (hCO == NULL) {
				std::cout << "OKAO_CO_CreateHandle() Error\n" << nRet << std::endl;
			}
			//face detection serttings
			hDT = OKAO_CreateDetection();
			if (hDT == NULL) {
				std::cout << "OKAO_CreateDetection() Error\n" << nRet << std::endl;
			}
			hDtResult = OKAO_CreateDtResult(35, 0);
			if (hDtResult == NULL) {
				std::cout << "OKAO_CreateDtResult() Error\n" << nRet << std::endl;
			}
			hCO = OKAO_CO_CreateHandle(NULL, NULL, (ALLOCATOR)malloc, free);
			if (hCO == NULL) {
				std::cout << "OKAO_CO_CreateHandle() Error\n" << nRet << std::endl;
			}
			/***********************************/
			/* Facial Parts Detection Settings */
			/***********************************/
			/* Creates Facial Parts Detection Handle */
			hPT = OKAO_PT_CreateHandle();
			if (hPT == NULL) {
				std::cout << "OKAO_PT_CreateHandle() Error\n" << nRet << std::endl;
			}
			/* Creates Facial Parts Detection Result Handle */
			hPtResult = OKAO_PT_CreateResultHandle();
			if (hPtResult == NULL) {
				std::cout << "OKAO_PT_CreateResultHandle() Error\n" << nRet << std::endl;
			}
			//smile degree handler
			hSM = OKAO_SM_CreateHandle();
			if (hSM == NULL) {
				std::cout << "OKAO_SM_CreateHandle() Error\n" << nRet << std::endl;
			}
			/* Creates Smile Degree Estimation Result Handle */
			hSmResult = OKAO_SM_CreateResultHandle();
			if (hSmResult == NULL) {
				std::cout << "OKAO_SM_CreateResultHandle() Error\n" << nRet << std::endl;
			}
			hEX = OKAO_EX_CreateHandle(hCO);
			if (hEX == NULL) {
				std::cout << "OKAO_EX_CreateHandle() Error\n" << nRet << std::endl;
			}
			/**************************************/
			/* Gaze and Blink Estimation Settings */
			/**************************************/
			/* Creates Gaze and Blink Estimation Handle */
			hGB = OKAO_GB_CreateHandle();
			if (hGB == NULL) {
				std::cout << "OKAO_GB_CreateHandle() Error\n" << nRet << std::endl;
			}
			/* Creates Gaze and Blink Estimation result Handle */
			hGbResult = OKAO_GB_CreateResultHandle();
			if (hGbResult == NULL) {
				std::cout << "OKAO_GB_CreateResultHandle() Error\n" << nRet << std::endl;
				//break;
			}
			//main while loop
			while ('q' != cv::waitKey(30))
			{
				cv::Mat image;


				webcam >> image;
				if (image.empty() == false)
				{
					cv::Mat grayImage;
					cv::cvtColor(image, grayImage, CV_BGR2GRAY); //converting the image to grey color
					nRet = OKAO_Detection(hDT, grayImage.ptr(), grayImage.cols, grayImage.rows, ACCURACY_NORMAL, hDtResult);

					if (nRet != OKAO_NORMAL) {
						std::cout << "OKAO_Detection() Error : \n" << nRet << std::endl;
						break;
					}
					int nCount = 0;
					nRet = OKAO_GetDtFaceCount(hDtResult, &nCount);
					char smiletext1[100] = "";
					char smiletext2[100] = "";
					char gazetext1[100] = "";
					char gazetext2[100] = "";
					char exptext1[100] = "";
					char exptext2[100] = "";

					char fpsbps[100];
					char fpsbps1[100];
					char fpsbps2[100];
					char fpsbps2_2[100];

					bool sameuser;
					sameuser = false;
					for (int i = 0; i < nCount; ++i) {
						FACEINFO sFaceInfo = { 0 };
						OKAO_GetDtFaceInfo(hDtResult, i, &sFaceInfo);

						long usersep;



						usersep = (sFaceInfo.ptLeftBottom.x + sFaceInfo.ptRightBottom.x) / 2;
						if (usersep < 320)
						{
							if (userID != 2)
							{
								userID = 2;
								cv::rectangle(image, getCvRect(sFaceInfo), CV_RGB(0, 255, 0), 3);
								sameuser = false;

							}
							else
								sameuser = true;
						}

						else
						{
							if (userID != 1)
							{
								userID = 1;
								cv::rectangle(image, getCvRect(sFaceInfo), CV_RGB(255, 0, 0), 5);
								sameuser = false;
							}
							else
								sameuser = true;
						}
						if (sameuser == false)
						{

							nRet = OKAO_PT_SetPositionFromHandle(hPT, hDtResult, i);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_PT_SetPositionFromHandle() Error :\n" << nRet << std::endl;
								break;
							}
							/* Executes Facial Parts Detection */
							nRet = OKAO_PT_DetectPoint(hPT, grayImage.ptr(), grayImage.cols, grayImage.rows, hPtResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_PT_DetectPoint() Error : %d\n" << nRet << std::endl;
								break;
							}
							/* Gets Facial Parts Position Result */
							nRet = OKAO_PT_GetResult(hPtResult, PT_POINT_KIND_MAX, aptPoint, anConf);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_PT_GetResult() Error : %d\n" << nRet << std::endl;
								break;
							}
							nRet = OKAO_SM_SetPointFromHandle(hSM, hPtResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_SM_SetPointFromHandle() Error : " << nRet << std::endl;
								break;
							}

							/* Estimate the smile degree */
							nRet = OKAO_SM_Estimate(hSM, grayImage.ptr(), grayImage.cols, grayImage.rows, hSmResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_SM_Estimate() Error : %d\n" << nRet << std::endl;
								break;
							}
							/* Gets the estimated smile degree and its confidence level */
							nRet = OKAO_SM_GetResult(hSmResult, &nSmile, &nConfidence);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_SM_GetResult() Error : %d\n" << nRet << std::endl;
								break;
							}
							nRet = OKAO_PT_GetFaceDirection(hPtResult, &Fupdown, &Fleftright, &Froll);
							if (nRet != OKAO_NORMAL){
								std::cout << "OKAO_PT_GetResult() Error : %d\n" << nRet << std::endl;
								break;

							}

							/*****************************/
							/* Gaze and Blink Estimation */
							/*****************************/
							/* Sets facial parts position from PT result handle */
							nRet = OKAO_GB_SetPointFromHandle(hGB, hPtResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_GB_SetPointFromHandle() Error : %d\n" << nRet << std::endl;
								break;
							}
							/* Execute Gaze and Blink Estimation */
							nRet = OKAO_GB_Estimate(hGB, grayImage.ptr(), grayImage.cols, grayImage.rows, hGbResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_GB_Estimate() Error : %d\n" << nRet << std::endl;
								break;
							}
							/* Gets the Gaze estimation result */
							nRet = OKAO_GB_GetGazeDirection(hGbResult, &nGazeLeftRight, &nGazeUpDown);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_GB_GetGazeDirection() Error : %d\n" << nRet << std::endl;
								break;
							}
							/* Gets the Blink estimation result */
							nRet = OKAO_GB_GetEyeCloseRatio(hGbResult, &nCloseRatioLeftEye, &nCloseRatioRightEye);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_GB_GetEyeCloseRatio() Error : %d\n" << nRet << std::endl;
								break;
							}
							/***************************/
							/* Expression Estimation   */
							/***************************/
							/* Sets the feature points for Expression Estimation from PT result handle */
							nRet = OKAO_EX_SetPointFromHandle(hEX, hPtResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_EX_SetPointFromHandle() Error : %d\n" << nRet << std::endl;
								break;
							}
							/* Estimate the Expression */
							stImage.apImage[eIMG_PLANAR_FIRST] = grayImage.ptr();
							stImage.apImage[eIMG_PLANAR_SECOND] = NULL;
							stImage.apImage[eIMG_PLANAR_THIRD] = NULL;
							stImage.nWidth = 640;
							stImage.nHeight = 480;
							stImage.unImageFormat = GRAY_Y0Y1Y2Y3;
							nRet = OKAO_EX_Estimate(hEX, stImage, &stExResult);
							if (nRet != OKAO_NORMAL) {
								std::cout << "OKAO_EX_Estimate() Error : %d\n" << nRet << std::endl;
								break;
							}
							if (scenario == 1)
							{
								
								userID = 1;
							}
							//temp buffers
							sprintf(fpsbps, "User:%2d Confidence:%3d Smile:%3d, CloseLeftEye:%d, CloseRightEye:%d", userID, nConfidence, nSmile, nCloseRatioLeftEye, nCloseRatioRightEye);
							sprintf(fpsbps1, "Neutral:%3d Happiness:%3d Surprise:%3d Fear:%3d Anger:%3d Disgust:%3d Sadness:%3d", stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6]);
							sprintf(fpsbps2, "GazeLeftRight:%d, GazeUpDown:%d, FaceLeftRight:%d, FaceUpDown:%d", leftright, updown, nFleftright, nFupdown);
							sprintf(fpsbps2_2, "GazeLeftRight:%d, GazeUpDown:%d, FaceLeftRight:%d, FaceUpDown:%d", leftright2, updown2, nFleftright2, nFupdown2);
							
							
							if (userID == 1)
							{

								timess++;
								INT32 hor = 0;
								INT32 ver = 0;
								hor = (sFaceInfo.ptLeftTop.x + sFaceInfo.ptRightTop.x) / 2; //face position on X
								ver = (sFaceInfo.ptLeftTop.y + sFaceInfo.ptLeftBottom.y) / 2; //face position on Y


								hor = hor - (640 / 2);
								if (timess < 4)
								{
									if (nGazeUpDown>0) //up
										up = up + nGazeUpDown;
									if (nGazeUpDown <= 0) //down
										down = down + nGazeUpDown;
									if (nGazeLeftRight > 0)
										right = right + nGazeLeftRight;
									if (nGazeLeftRight <= 0) //left
										left = left + nGazeLeftRight;

									if (Fupdown > 0) //up
										fup = fup + Fupdown;
									if (Fupdown <= 0) //down
										fdown = fdown + Fupdown;
									if (Fleftright > 0)
										fright = fright + Fleftright;
									if (Fleftright <= 0) //left
										fleft = fleft + Fleftright;

								}
								else
								{
									timess = 0;
									up = up / 3;
									down = down / 3;
									left = left / 3;
									right = right / 3;
									updown = up + down;
									leftright = left + right;

									fup = fup / 3;
									fdown = fdown / 3;
									fleft = fleft / 3;
									fright = fright / 3;
									nFupdown = fup + fdown;
									nFleftright = fleft + fright;
									float vectorX = 0;
									float vectorY = 0;
									string a10="none";
									if ((nCloseRatioLeftEye<500 && nCloseRatioRightEye<500) && (nFupdown>-15 && nFupdown < 15 && nFleftright > -20 && nFleftright<20))
									{
										vectorX = (float)(nFleftright + leftright);
										vectorY = (float)(nFupdown + updown);
										vectorX = vectorX + (float)(hor * 2) / 40;
									}
									else
									{
										vectorX = (float)nFleftright;
										vectorY = (float)nFupdown;
									}
									if (vectorY < 16) //then gazing at screen for sure
									if (vectorX < 0) //gaze right screen
										a10 = "screenR";
									else
										a10 = "screenL";
									else //robot or other
									{
										if (vectorX>-20 && vectorX<20 && vectorY <= 30) //then robot
											a10 = "robot";
										else
											a10 = "else";
									}

									up = 0;
									down = 0;
									left = 0;
									right = 0;
									fup = 0;
									fdown = 0;
									fleft = 0;
									fright = 0;
									char pipetext[250] = "";
									//sprintf(pipetext, "User:%1d,Confidence:%3d,Smile:%3d,Neutral:%3d,Happiness:%3d,Surprise:%3d,Fear:%3d,Anger:%3d,Disgust:%3d,Sadness:%3d,GazeLeftRight=%3d,GazeUpDown=%3d,LeftEye=%3d,RightEye=%3d,Fupdown=%3d,Fleftright=%3d", userID, nConfidence, nSmile, stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6], nGazeLeftRight, nGazeUpDown, nCloseRatioLeftEye, nCloseRatioRightEye,Fupdown,Fleftright);
									sprintf(pipetext, "User,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%s", userID, nConfidence, nSmile, stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6], nFupdown, nFleftright,a10);
									sendpipe(pipetext);
								}

								strcat(smiletext1, fpsbps);
								strcat(gazetext1, fpsbps2);
							}
							if (userID == 2)
							{
								timess2++;
								INT32 hor = 0;
								INT32 ver = 0;
								hor = (sFaceInfo.ptLeftTop.x + sFaceInfo.ptRightTop.x) / 2;
								ver = (sFaceInfo.ptLeftTop.y + sFaceInfo.ptLeftBottom.y) / 2;

								hor = hor - (640 / 2);

								if (timess2 < 4)
								{
									if (nGazeUpDown>0) //up
										up2 = up2 + nGazeUpDown;
									if (nGazeUpDown <= 0) //down
										down2 = down2 + nGazeUpDown;
									if (nGazeLeftRight > 0)
										right2 = right2 + nGazeLeftRight;
									if (nGazeLeftRight <= 0) //left
										left2 = left2 + nGazeLeftRight;

									if (Fupdown > 0) //up
										fup2 = fup2 + Fupdown;
									if (Fupdown <= 0) //down
										fdown2 = fdown2 + Fupdown;
									if (Fleftright > 0)
										fright2 = fright2 + Fleftright;
									if (Fleftright <= 0) //left
										fleft2 = fleft2 + Fleftright;


								}
								else
								{
									timess2 = 0;
									up2 = up2 / 3;
									down2 = down2 / 3;
									left2 = left2 / 3;
									right2 = right2 / 3;
									updown2 = up2 + down2;
									leftright2 = left2 + right2;

									fup2 = fup2 / 3;
									fdown2 = fdown2 / 3;
									fleft2 = fleft2 / 3;
									fright2 = fright2 / 3;
									nFupdown2 = fup2 + fdown2;
									nFleftright2 = fleft2 + fright2;

									float vectorX = 0;
									float vectorY = 0;
									string a10;
									if ((nCloseRatioLeftEye<500 && nCloseRatioRightEye<500) && (nFupdown2>-15 && nFupdown2 < 15 && nFleftright2 > -20 && nFleftright2<20))
									{
										vectorX = (float)(nFleftright2 + leftright2);
										vectorY = (float)(nFupdown2 + updown2);
										vectorX = vectorX + (float)(hor*2) / 40;
									}
									else
									{
										vectorX = (float)nFleftright2;
										vectorY = (float)nFupdown2;
									}
									if (vectorY < 16) //then gazing at screen for sure
									if (vectorX < 0) //gaze right screen
										a10 = "screenR";
									else
										a10 = "screenL";
									else //robot or other
									{
										if (vectorX>-20 && vectorX<20 && vectorY <= 30) //then robot
											a10 = "robot";
										else
											a10 = "else";
									}
									up2 = 0;
									down2 = 0;
									left2 = 0;
									right2 = 0;
									fup2 = 0;
									fdown2 = 0;
									fleft2 = 0;
									fright2 = 0;
									char pipetext[250] = "";
									//sprintf(pipetext, "User:%1d,Confidence:%3d,Smile:%3d,Neutral:%3d,Happiness:%3d,Surprise:%3d,Fear:%3d,Anger:%3d,Disgust:%3d,Sadness:%3d,GazeLeftRight=%3d,GazeUpDown=%3d,LeftEye=%3d,RightEye=%3d,Fupdown=%3d,Fleftright=%3d", userID, nConfidence, nSmile, stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6], nGazeLeftRight, nGazeUpDown, nCloseRatioLeftEye, nCloseRatioRightEye,Fupdown,Fleftright);
									sprintf(pipetext, "User,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%s", userID, nConfidence, nSmile, stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6], Fupdown, Fleftright,a10);
									sendpipe(pipetext);
								}
								strcat(smiletext2, fpsbps);
								strcat(gazetext2, fpsbps2_2);

							}

							
							//printing information on the screen

							cv::putText(image, smiletext1, cv::Point(10, 20), cv::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(255, 0, 200));
							cv::putText(image, smiletext2, cv::Point(10, 60), cv::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(155, 0, 200));
							cv::putText(image, gazetext1, cv::Point(10, 40), cv::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(155, 0, 200));
							cv::putText(image, gazetext2, cv::Point(10, 80), cv::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(155, 0, 200));

						}
						
						}
						userID = 0;
						sameuser = false;

					
					}
				
					
				try{

					cv::imshow("", image);
					

				}
				catch (exception e){ break; }
			}

			/********************************/
			/* Handle Deletion              */
			/********************************/
			/* Deletes Smile Degree Estimation Handle */
			if (hSM != NULL) {
				OKAO_SM_DeleteHandle(hSM);
			}
			/* Deletes Smile Degree Estimation Result Handle */
			if (hSmResult != NULL) {
				OKAO_SM_DeleteResultHandle(hSmResult);
			}
			/* Deletes Facial Parts Detection Handle */
			if (hPT != NULL) {
				OKAO_PT_DeleteHandle(hPT);
			}
			/* Deletes Facial Parts Detection Result Handle */
			if (hPtResult != NULL) {
				OKAO_PT_DeleteResultHandle(hPtResult);
			}
			/* Deletes Face Detection handle */
			if (hDT != NULL) {
				OKAO_DeleteDetection(hDT);
			}
			/* Deletes Face Detection result handle */
			if (hDtResult != NULL) {
				OKAO_DeleteDtResult(hDtResult);
			}

			if (hCO != NULL) {
				OKAO_CO_DeleteHandle(hCO);
			}

	

		}			
		
	
		
	
