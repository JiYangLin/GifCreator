import imageio
import sys
import os, os.path
from PIL import Image, ImageSequence

def ImgeToGif(imageURI):
    staticImages = []
    for filename in os.listdir(imageURI):
        staticImages.append(imageio.imread(imageURI+'\\'+filename))
        print(filename)

    writerKwargs = {'duration': '0.1'}
    imageWriter = imageio.get_writer(imageURI+'\\'+'AnimatedGif.gif', 'GIF', 'I', **writerKwargs)
    for image in staticImages:
    	imageWriter.append_data(image)
    imageWriter.close()
    print("生成结束" + imageURI+'\\'+'AnimatedGif.gif')
    

ImgeToGif(r"D:\AAA")