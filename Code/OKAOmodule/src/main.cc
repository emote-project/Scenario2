#include "opencv2/opencv.hpp"
#include <stdio.h>
#include <string.h>
#include "iostream"
#include "fstream"


HANDLE hPipe1;
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
			
			
			//cv::VideoCapture webcam("c:\\video_35.wmv");
			
			cv::VideoCapture webcam(selectedCamera);


			
			INT32 nIndex = 0;                   /* Index of face detection */
			
			INT32 nGazeLeftRight;           /* Right and left estimation result angle for gaze */
			INT32 nGazeUpDown;              /* Upper and lower estimation result angle for gaze */
			INT32 nCloseRatioLeftEye;       /* left eye estimation result for blink */
			INT32 nCloseRatioRightEye;      /* Right eye estimation result for blink */
			INT32 nSmile;
			INT32 userID;
			INT32 nConfidence;

			INT32 nMaxFaceNumber = 1;
			INT32 nMaxSwapNumber = 0;
			

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
			
			
			
			//main while loop
			while ('q' != cv::waitKey(30))
			{
				sendpipe("User");
				if (webcam.isOpened()==true) {
				
					cv::Mat image;
					webcam >> image;

					if ((image.empty() == false) & (image.rows>0))
					{
						cv::Mat grayImage;


						cv::cvtColor(image, grayImage, CV_BGR2GRAY); //converting the image to grey color
						
							int nCount = 0;
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
							

							long usersep;



							if (usersep < 320)
							{
								if (userID != 2)
								{
									userID = 2;
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
									sameuser = false;
								}
								else
									sameuser = true;
							}
							if (sameuser == false)
							{

								
								
								
								
								
								if (scenario == 1)
								{

									userID = 1;
								}
								//temp buffers
								sprintf(fpsbps, "User:%2d Confidence:%3d Smile:%3d, CloseLeftEye:%d, CloseRightEye:%d", userID, nConfidence, nSmile, nCloseRatioLeftEye, nCloseRatioRightEye);
						//		sprintf(fpsbps1, "Neutral:%3d Happiness:%3d Surprise:%3d Fear:%3d Anger:%3d Disgust:%3d Sadness:%3d", stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6]);
								sprintf(fpsbps2, "GazeLeftRight:%d, GazeUpDown:%d, FaceLeftRight:%d, FaceUpDown:%d", leftright, updown, nFleftright, nFupdown);
								sprintf(fpsbps2_2, "GazeLeftRight:%d, GazeUpDown:%d, FaceLeftRight:%d, FaceUpDown:%d", leftright2, updown2, nFleftright2, nFupdown2);


								if (userID == 1)
								{

									timess++;
									INT32 hor = 0;
									INT32 ver = 0;
								//	hor = (sFaceInfo.ptLeftTop.x + sFaceInfo.ptRightTop.x) / 2; //face position on X
								//	ver = (sFaceInfo.ptLeftTop.y + sFaceInfo.ptLeftBottom.y) / 2; //face position on Y


									hor = hor - (640 / 2);
									if (timess < 4)
									{
										if (nGazeUpDown > 0) //up
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
										string a10 = "none";
										if ((nCloseRatioLeftEye < 500 && nCloseRatioRightEye<500) && (nFupdown>-15 && nFupdown < 15 && nFleftright > -20 && nFleftright < 20))
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
											if (vectorX > -20 && vectorX < 20 && vectorY <= 30) //then robot
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
								//		sprintf(pipetext, "User,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%s", userID, nConfidence, nSmile, stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6], nFupdown, nFleftright, a10);
										sendpipe(pipetext);
										nConfidence = 0;
										nSmile = 0;
						//				stExResult.anScore[0] = 0;
						//				stExResult.anScore[0] = 1;
						//				stExResult.anScore[0] = 2;
						//				stExResult.anScore[0] = 3;
						//				stExResult.anScore[0] = 4;
						//				stExResult.anScore[0] = 5;
						//				stExResult.anScore[0] = 6;


									}

									strcat(smiletext1, fpsbps);
									strcat(gazetext1, fpsbps2);
								}
								if (userID == 2)
								{
									timess2++;
									INT32 hor = 0;
									INT32 ver = 0;
						//			hor = (sFaceInfo.ptLeftTop.x + sFaceInfo.ptRightTop.x) / 2;
						//			ver = (sFaceInfo.ptLeftTop.y + sFaceInfo.ptLeftBottom.y) / 2;

									hor = hor - (640 / 2);

									if (timess2 < 4)
									{
										if (nGazeUpDown > 0) //up
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
										if ((nCloseRatioLeftEye < 500 && nCloseRatioRightEye<500) && (nFupdown2>-15 && nFupdown2 < 15 && nFleftright2 > -20 && nFleftright2 < 20))
										{
											vectorX = (float)(nFleftright2 + leftright2);
											vectorY = (float)(nFupdown2 + updown2);
											vectorX = vectorX + (float)(hor * 2) / 40;
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
											if (vectorX > -20 && vectorX < 20 && vectorY <= 30) //then robot
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
								//		sprintf(pipetext, "User,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%s", userID, nConfidence, nSmile, stExResult.anScore[0], stExResult.anScore[1], stExResult.anScore[2], stExResult.anScore[3], stExResult.anScore[4], stExResult.anScore[5], stExResult.anScore[6], Fupdown, Fleftright, a10);
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
					if (image.empty() == false)
						cv::imshow("", image);
					else
					{
						webcam.release();
						webcam.open(0);
					}
				}
			}


	

		}			
		
	
		
	
