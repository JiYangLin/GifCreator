import imageio
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

## ImgeToGif(r"D:\HB\素材\安全车-标线识别.wmv-20200102101756")


import imageio
from PIL import Image, ImageSequence
def ResizeGif(ImgName,w,h):
    im = Image.open(ImgName)
    staticImages = []
    i = 0
    for frame in ImageSequence.Iterator(im):
        frame = frame.convert('RGB')
        frame.thumbnail((w, h))
        frame.save("temp.jpg")
        staticImages.append(imageio.imread("temp.jpg"))
        i = i+1
        print("parse img:%d"%(i))
    writerKwargs = {'duration': '0.1'}
    pathname = ImgName+ '-ret.gif'
    imageWriter = imageio.get_writer(pathname, 'GIF', 'I', **writerKwargs)
    for image in staticImages:
    	imageWriter.append_data(image)
    print("save img..." + pathname)
    imageWriter.close()
    print("save finished! ")  

ImgName = r"D:\\HB\\安全车\\PC\\标线自动识别.gif"
ResizeGif(ImgName,500,1050)
