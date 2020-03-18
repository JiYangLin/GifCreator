import PythonMagick
import sys

if __name__ == "__main__":
    try:
        img = PythonMagick.Image(sys.argv[1])
        img.sample(sys.argv[2])
        img.write(sys.argv[1] + "--result.ico")
    except Exception as e:
        print(e,flush=True)

