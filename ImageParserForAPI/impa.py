from queue import Full
import sys
sys.path.append('C:\\Users\\Aethelhelm\\AppData\\Local\\Programs\\Python\\Python311\\Lib\\site-packages')

import numpy
import cv2
import pytesseract
import os


def thin_font(image):
	kernel = numpy.ones((2,2),numpy.uint8)
	erode = cv2.erode(image, kernel, iterations=1)
	return (erode)

class ImageParser:
	def __init__(self, tesspath):
		self.resize_scale = 5
		TESS_PATH = tesspath + "tessbin"
		os.environ["TESSDATA_PREFIX"] = TESS_PATH + "\\tessdata\\"
		pytesseract.pytesseract.tesseract_cmd = TESS_PATH + "\\tesseract"

	def parse_screenshot(self, fullscreen_image, wtype, version):
		print(version)
		crops = self.crop_fullscreen(fullscreen_image,wtype)
		for crop in crops:
			print("=" * 5, crop[0], "=" * 5)
			data = self.text_from_image(crop[1],crop[0])
			print(data + "\n")

	def crop_fullscreen(self, fullscreen_image, wtype):
		if type(fullscreen_image) is str:
			#print(fullscreen_image)
			print("Does the file exist? " + str(os.path.exists(fullscreen_image)));
			fullscreen_image = cv2.imread(fullscreen_image)
		crops = []
		if wtype == 1: #gun
			#advanced menu crops:
			a_crop = fullscreen_image[280:520, 1250:1520]
			b_crop = fullscreen_image[100:320, 1250:1520]
			w_crop = fullscreen_image[540:760, 1250:1520]
			m_crop = fullscreen_image[730:950, 1250:1520]
			#main crops
			r_crop = fullscreen_image[100:320, 1540:1810]
			d_crop = fullscreen_image[320:540, 1540:1810]
			f_crop = fullscreen_image[540:760, 1540:1810]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
		elif wtype == 2: #grenade
			r_crop = fullscreen_image[5:225, 1540:1810]
			d_crop = fullscreen_image[230:450, 1540:1810]

			crops.append(("DamageInfo",d_crop))
			crops.append(("RankInfo",r_crop))
		elif wtype == 3: #melee
			r_crop = fullscreen_image[80:300, 1540:1810]
			a_crop = fullscreen_image[290:510, 1540:1810]

			crops.append(("RankInfo",r_crop))
			crops.append(("AnimationSpeeds",a_crop))


		return crops
	
	
	def text_from_image(self, image, cropname):
		if type(image) is str:
			image = cv2.imread(image)
		if image.size == 0:
			return "[OCR] Empty Image"
		bw = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)


		dim = (int(bw.shape[0] * (self.resize_scale + 4)), int(bw.shape[1] * self.resize_scale))
		resize = cv2.resize(bw, dim, interpolation=cv2.INTER_AREA)

		#process = unsharp_mask(resize)
		process1 = cv2.cvtColor( resize, cv2.COLOR_GRAY2BGR)

		lab= cv2.cvtColor(process1, cv2.COLOR_BGR2LAB)
		l_channel, a, b = cv2.split(lab)

		# Applying CLAHE to L-channel
		# feel free to try different values for the limit and grid size:
		clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8,8))
		cl = clahe.apply(l_channel)

		# merge the CLAHE enhanced L-channel with the a and b channel
		limg = cv2.merge((cl,a,b))

		# Converting image from LAB Color model to BGR color spcae
		enhanced_img = cv2.cvtColor(limg, cv2.COLOR_LAB2BGR)
		process2 = cv2.cvtColor(enhanced_img, cv2.COLOR_BGR2GRAY)
		sharpened_img = cv2.bilateralFilter(process2,9,75,75)
		thresh,im_bw = cv2.threshold(sharpened_img, 140, 255, cv2.THRESH_BINARY )
		eroded = thin_font(im_bw)
		if cropname == "DamageInfo":
			return pytesseract.image_to_string(eroded, config="--psm 11")
		elif cropname == "FireInfo":
			return pytesseract.image_to_string(sharpened_img, config="--psm 4")
		filename = "pythoncvtesting/" + cropname + "_eroded.png"
		filename2 = "pythoncvtesting/" + cropname + "_sharpened.png"
		cv2.imwrite(filename, eroded)
		cv2.imwrite(filename2,sharpened_img)

		data = pytesseract.image_to_string(sharpened_img, config="--psm 4")
		return data



#command arguments:
# 1: tessbin location
# 2: image location
# 3: type of weapon (1 = gun, 2 = grenade, 3 = melee)
tessbinfilepath = ""

if len(sys.argv) == 1:
	print("i force fed myself olives for a month, i hated it, but now i love olives")
else:
	if sys.argv[1] == "--help" or sys.argv[1] == "-h":
		print("""
Image Parser for Phantom Forces Database
COMMAND USAGE: imp [OPTIONS] (TESSBINPATH) FILEPATH TYPE (VERSION)
		
OPTIONS:
	-h  --help  Displays this help message, and exits
	-c          Tessbin folder exists in the current directory (TESSBINPATH is ignored)
	-f          Tessbin folder exists in another directory (specified by TESSBINPATH)

TESSBINPATH: path to /tessbin/ (important for pytesseract)

FILEPATH: path to file to be processed

TYPES: Specifies the type of weapon to be processed
	1 = Gun
	2 = Grenade
	3 = Melee

VERSION: specifies the version of Phantom Forces (completely optional, but recommended for archival purposes)
		""")
		sys.exit()
	elif sys.argv[1] == "-c":
		if len(sys.argv) == 4:
			pa = ImageParser("")
			pa.parse_screenshot(sys.argv[2],int(sys.argv[3]),"") #tessbinpath becomes filepath, filepath becomes type
		elif len(sys.argv) == 5:
			pa = ImageParser("")
			pa.parse_screenshot(sys.argv[2], int(sys.argv[3]), sys.argv[4])#tessbinpath becomes filepath, filepath becomes type, type becomes version
		elif len(sys.argv) > 5:
			print("Too many parameters. Expected 4: imp -c FILEPATH TYPE (VERSION)")
		else:
			print("Too few parameters. Expected 4: imp -c FILEPATH TYPE (VERSION)")
			sys.exit()
	elif sys.argv[1] == "-f":
		if len(sys.argv) == 5:
			pa = ImageParser(sys.argv[2])
			pa.parse_screenshot(sys.argv[3],int(sys.argv[4])) 
		elif len(sys.argv) == 6:
			pa = ImageParser(sys.argv[2])
			pa.parse_screenshot(sys.argv[3], int(sys.argv[4]), sys.argv[5])
		elif len(sys.argv) > 6:
			print("Too many parameters. Expected 5: imp -f TESSBINPATH FILEPATH TYPE (VERSION)")
		else:
			print("Too few parameters. Expected 5: imp -f TESSBINPATH FILEPATH TYPE (VERSION)")
			sys.exit()
	else:
		print("i force fed myself olives for a month, i hated it, but now i love olives")


#pyinstaller command: py -m PyInstaller --onefile --paths=C:\Users\peter\AppData\Local\Programs\Python\Python312 imp.py
	

"i force fed myself olives for a month, i hated it, but now i love olives" #quote by dw harder


#pa = ImageParser()
#pa.parse_screenshot(sys.argv[3],int(sys.argv[4]))
#pa.parse_screenshot("C:\\Users\\Aethelhelm\\Programming\\source\\repos\\PFDB_API\\ImageParserForAPI\\13_2.png",2)
