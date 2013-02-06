//
//  BrickRecognitionLib.h
//  BrickRecognitionLib
//
//  Created by EyeCue EyeCue on 9/19/12.
//  Copyright (c) 2012 EyeCue. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface BrickRecognitionLib : NSObject
{
    bool licenceExpired;
}

-(NSString *)version;

/*
 -(int) detectMatRedBuffer:(unsigned char *)R greenBuffer:(unsigned char *)G  blueBuffer: (unsigned char *)B;
 
 Description:
 This function is responsible for detecting the mat based on the RGB image components.
 
 Parameters: 
 RedBuffer:(unsigned char *)R   -   (in) The red component buffer of the input image.
 greenBuffer:(unsigned char *)G -   (in) The green component buffer of the input image.
 blueBuffer:(unsigned char *)B  -   (in) The blue component buffer of the input image.
 
 Return value: 
 int    -   A flag indecting if a Mat was found. Possible results 0: no matt was found, 1: success mat v.4 was found. 2: success mat v.5 was found 
 */
-(int) detectMatRedBuffer:(unsigned char *)R greenBuffer:(unsigned char *)G  blueBuffer: (unsigned char *)B;

/*
 -(int) ExtractModelX:(unsigned char *)modelX modelY:(unsigned char *)modelY modelColor:(unsigned char *)modelColor;

 Description:
 This function extracts the model from the mat that was detected using the detectMatRedBuffer:greenBuffer:blueBuffer function. 
 Some of the parameters for this function are passed directly from the detectMatRedBuffer:greenBuffer:blueBuffer.
 It fills the 3 input buffers with the values of x position, y position and color of each brick that was found. 
 
 Parameters:
 ModelX:(unsigned char *)modelX         -   (in/out) Pointer to an array with the lengh of max number of possible bricks (currenlty 15*15).
                                            The funciton fills the array with the X postion values for each brick found.
 ModelY:(unsigned char *)modelY         -   (in/out) Pointer to an array with the lengh of max number of possible bricks (currenlty 15*15).
                                            The funciton fills the array with the Y postion values for each brick found.
 ModelColor:(unsigned char *)modelColor -   (in/out) Pointer to an array with the lengh of max number of possible bricks (currenlty 15*15).
                                            The funciton fills the array with the color values for each brick found.
 
 Return value: 
 -(int) - The number of bricks that where found.
 */
-(int) ExtractModelX:(unsigned char *)modelX modelY:(unsigned char *)modelY modelColor:(unsigned char *)modelColor brickType:(unsigned char *)brickType;

/*
 -(void)    modelRecognitionModelX:(unsigned char *)modelX 
                            modelY:(unsigned char *)modelY 
                            modelColor:(unsigned char *)modelColor
                            modelCount:(int) modelCnt
                            resultsArray:(unsigned char *)results
                            colorHistogram:(int *)colorHistogram
 
 Description:
 This function calculate the accuracy when playing in classic/silhoutte/memory. This function should be called after 
 the detectMatRedBuffer:greenBuffer:blueBuffer function succeded in finding a matt.
 
 Parameters: 
 ModelX:(unsigned char *)modelX         -   (in) An array that holds the X position of each brick in the target model.
 ModelY:(unsigned char *)modelY         -   (in) An array that holds the Y position of each brick in the target model.
 ModelColor:(unsigned char *)modelColor -   (in) An array that holds the color of each brick in the target model.
 modelCount:(int) modelCnt              -   (in) holds the number of bricks in the target model.
 resultsArray:(unsigned char *)results  -   (in/out) a pointer to an array with the lengh of 4. The function fills he results of the comparison between 
                                            the target model and the acual model detected in to the array. results[0] = the number of bricks in the captured
                                            model that are in the same position and color as the target model.
                                            results[1] = the number of brick in the captured model that are outside the silhoutte of the 
                                            target model. results[2] = not used. results[3] = number of bricks in the capture model
                                            with the wrong color compared to the target model.
 colorHistogram:(int *)colorHistogram   -   (in/out) a pointer to an array with the lengh of 6. The function fills each cell in the array
                                            with the number of bricks found with a specific color. 
                                            colorHistogram[0] = the number of white bricks. colorHistogram[1] = the number of black bricks
                                            colorHistogram[2] = the number of red bricks. colorHistogram[3] = the number of yellow
                                            colorHistogram[4] = the number of green bricks. colorHistogram[5] = the number of blue bricks.
 
 
 Return value:
 N/A 
 */

-(void) modelRecognitionModelX:(unsigned char *)modelX 
                        modelY:(unsigned char *)modelY 
                    modelColor:(unsigned char *)modelColor
                    modelCount:(int) modelCnt
                  resultsArray:(unsigned char *)results
                colorHistogram:(int *)colorHistogram;

/*
 -(void)    modelRecognitionModelX:(unsigned char *)modelX 
 modelY:(unsigned char *)modelY 
 modelColor:(unsigned char *)modelColor
 modelCount:(int) modelCnt
 resultsArray:(unsigned char *)results
 colorHistogram:(int *)colorHistogram
 
 Description:
 This function calculate the accuracy when playing in classic/silhoutte/memory. This function should be called after 
 the detectMatRedBuffer:greenBuffer:blueBuffer function succeded in finding a matt.
 
 Parameters: 
 ModelX:(unsigned char *)modelX         -   (in) An array that holds the X position of each brick in the target model.
 ModelY:(unsigned char *)modelY         -   (in) An array that holds the Y position of each brick in the target model.
 ModelColor:(unsigned char *)modelColor -   (in) An array that holds the color of each brick in the target model.
 modelCount:(int) modelCnt              -   (in) holds the number of bricks in the target model.
 resultsArray:(unsigned char *)results  -   (in/out) a pointer to an array with the lengh of 4. The function fills he results of the comparison between 
                                            the target model and the acual model detected in to the array. results[0] = the number of bricks in the captured
                                            model that are in the same position and color as the target model.
                                            results[1] = the number of brick in the captured model that are outside the silhoutte of the 
                                            target model. results[2] = not used. results[3] = number of bricks in the capture model
                                            with the wrong color compared to the target model.
 colorHistogram:(int *)colorHistogram   -   (in/out) a pointer to an array with the lengh of 6. The function fills each cell in the array
                                            with the number of bricks found with a specific color. 
                                            colorHistogram[0] = the number of white bricks. colorHistogram[1] = the number of black bricks
                                            colorHistogram[2] = the number of red bricks. colorHistogram[3] = the number of yellow
                                            colorHistogram[4] = the number of green bricks. colorHistogram[5] = the number of blue bricks.
 mode:(unsigned char) mode              -   (in) Set the game mode. 0 is regular, 1 is silhouette, 2 is shape finder
 shapeX:(unsigned char *)shapeX         -   (in) Array that holds the X position of each brick for the shape we want to find. 
                                            Used only in shape finder.
 shapeY:(unsigned char *)shapeY         -   (in) Array that holds the Y position of each brick for the shape we want to find. 
                                            Used only in shape finder
 shapeCount:(unsigned char *)shapeCnt   -   (in) Array that holds the number of bricks in the shape we want to find 
                                            Used only in shape finder
 
 Return value:
 N/A 
 */

-(void) modelRecognitionWithShapeModelX:(unsigned char *)modelX 
                                 modelY:(unsigned char *)modelY 
                             modelColor:(unsigned char *)modelColor
                             modelCount:(int) modelCnt
                           resultsArray:(unsigned char *)results
                         colorHistogram:(int *)colorHistogram
                                   mode:(unsigned char) mode
                                 shapeX:(unsigned char *)shapeX
                                 shapeY:(unsigned char *)shapeY
                             shapeCount:(int) shapeCnt;
@end
