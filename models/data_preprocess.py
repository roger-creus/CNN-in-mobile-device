import numpy as np # linear algebra
import pandas as pd # data processing, CSV file I/O (e.g. pd.read_csv)

import torch
import torchvision
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
from torchvision import transforms

import matplotlib.pyplot as plt
import csv

import cv2

from PIL import Image
from PIL import ImageFilter

import os
import shutil


def readTrafficSigns(rootpath):
    '''Reads traffic sign data for German Traffic Sign Recognition Benchmark.

    Arguments: path to the traffic sign data, for example './GTSRB/Training'
    Returns:   list of images, list of corresponding labels'''
    images = [] # images
    labels = [] # corresponding labels
    # loop over all 42 classes
    for c in range(0,43):
        prefix = rootpath + '/' + format(c, '05d') + '/' # subdirectory for class
        gtFile = open(prefix + 'GT-'+ format(c, '05d') + '.csv') # annotations file
        gtReader = csv.reader(gtFile, delimiter=';') # csv parser for annotations file
        next(gtReader) # skip header
        # loop over all images in current annotations file
        for row in gtReader:
            img_read = cv2.imread((prefix + row[0]))
            #CHANGE COLOR SPACE
            img_color = cv2.cvtColor(img_read, cv2.COLOR_BGR2RGB)
            
            
            #img_blur = cv2.GaussianBlur(img_color,(5, 5),0)
            
            images.append(img_color) 
            
            #DATA AUGMENTATION
            images.append(cv2.rotate(img_color, cv2.ROTATE_90_CLOCKWISE))
            images.append(cv2.rotate(img_color, cv2.ROTATE_90_COUNTERCLOCKWISE))
            images.append(cv2.flip(img_color, 0))
            images.append(cv2.flip(img_color, 1))
            images.append(cv2.flip(img_color, -1))
            
            labels.append(row[7])
            labels.append(row[7])
            labels.append(row[7])
            labels.append(row[7])
            labels.append(row[7])
            labels.append(row[7])
        gtFile.close()
    return images, labels


def main():
	data = readTrafficSigns("./Images")
	GT_testset = pd.read_csv("../input/trafficsign-gttestdata/GT-final_test.csv", sep=";")
	
	for i in range(0,43):
		os.mkdir("./000" + str(i))
	
	for img_id in GT_testset["Filename"]:    
    	shutil.copy('../input/trafficsign-test-data/GTSRB/Final_Test/Images/' + str(img_id), './' + str(GT_testset[GT_testset["Filename"] == img_id].ClassId.item()))
  

main()