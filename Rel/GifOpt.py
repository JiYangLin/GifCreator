import imageio
import sys
import os, os.path
from PIL import Image, ImageSequence


def ImgeToGif(imageURI):
    staticImages = []
    for filename in os.listdir(imageURI):
        staticImages.append(imageio.imread(imageURI+'\\'+filename))
        print(filename,flush=True)

    writerKwargs = {'duration': '0.1'}
    imageWriter = imageio.get_writer(imageURI+'--Gif.gif', 'GIF', 'I', **writerKwargs)
    for image in staticImages:
    	imageWriter.append_data(image)
    imageWriter.close()
def ResizeGif(ImgName,w,h):
    im = Image.open(ImgName)
    staticImages = []
    for frame in ImageSequence.Iterator(im):
        frame = frame.convert('RGB')
        frame.thumbnail((w, h))
        frame.save("temp.jpg")
        staticImages.append(imageio.imread("temp.jpg"))
    writerKwargs = {'duration': '0.1'}
    imageWriter = imageio.get_writer(ImgName+ '--resize.gif', 'GIF', 'I', **writerKwargs)
    for image in staticImages:
    	imageWriter.append_data(image)
    imageWriter.close()  

if __name__ == "__main__":
    try:
        if int(sys.argv[1]) == 0:
            ImgeToGif(sys.argv[2])
        else:
            ResizeGif(sys.argv[2],int(sys.argv[3]),int(sys.argv[4]))
    except Exception as e:
        print(e,flush=True)