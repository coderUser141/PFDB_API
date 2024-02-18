from queue import Full
import sys
sys.path.append('C:\\Users\\Aethelhelm\\AppData\\Local\\Programs\\Python\\Python311\\Lib\\site-packages')

import numpy
import cv2
import pytesseract
import os

# Thin font function
def thin_font(image):
	kernel = numpy.ones((2,2),numpy.uint8)
	erode = cv2.erode(image, kernel, iterations=1)
	return (erode)

def testimagepath(image):
	if type(image) is str:
		#print(fullscreen_image)
		print("Does the file exist? " + str(os.path.exists(image)))
		image = cv2.imread(image)
	return image

def categoryweaponcrop(image, wtype, version):
	#220,180 -> 475,730
	if(os.path.isdir("\\cropped\\")):
		os.mkdir(os.getcwd() + "\\cropped\\")
	cpath = os.getcwd() + "\\cropped\\"
	image = testimagepath(image)
	crop = image[180:730, 220:475]
	for i in range(300):
		if i % 3 == 0:
			fi = cpath + str(wtype) + "_" + version + str(i) + ".png"
			if(os.path.exists(fi)):
				continue
			else:
				print(fi + "saved!")
				cv2.imwrite(fi, crop)
				break
	#cv2.imwrite("")

class ImageParser:
	def __init__(self, tesspath):
		self.resize_scale = 5
		TESS_PATH = tesspath + "tessbin"
		os.environ["TESSDATA_PREFIX"] = TESS_PATH + "\\tessdata\\"
		pytesseract.pytesseract.tesseract_cmd = TESS_PATH + "\\tesseract"

	def parse_screenshot(self, fullscreen_image, wtype, version):
		print("Version: " + version)
		crops = self.crop_fullscreen(fullscreen_image,wtype, version)
		for crop in crops:
			f = open("file.txt","a")
			print("=" * 5, crop[0], "=" * 5)
			data = self.text_from_image(crop[1],crop[0], version)
			print(data + "\n")
			f.write(data + "\n")
			f.close()

	# in regards to the crop for damage and ranges, possibly use the page segmentation mode that is ideal for one line for the range markers
	# additionally, a way to isolate the text would be very beneficial

	#version 9.0.2 and 9.0.3
	def cropv902(self, fullscreen_image, wtype, crops):
		if wtype == 1: #primary
			#advanced menu crops:
			a_crop = fullscreen_image[280:520, 1250:1520]
			b_crop = fullscreen_image[100:320, 1250:1520]
			w_crop = fullscreen_image[540:760, 1250:1520]
			m_crop = fullscreen_image[730:950, 1250:1520]
			#main crops
			r_crop = fullscreen_image[100:320, 1540:1810]
			d_crop = fullscreen_image[320:425, 1540:1810]
			f_crop = fullscreen_image[450:670, 1540:1810]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
		elif wtype == 2: #secondary
			#advanced menu crops:
			a_crop = fullscreen_image[280:520, 1250:1520]
			b_crop = fullscreen_image[100:320, 1250:1520]
			w_crop = fullscreen_image[540:760, 1250:1520]
			m_crop = fullscreen_image[730:950, 1250:1520]
			#main crops
			r_crop = fullscreen_image[100:320, 1540:1810]
			d_crop = fullscreen_image[295:397, 1540:1810]
			f_crop = fullscreen_image[450:670, 1540:1810]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
		elif wtype == 3: #grenade
			r_crop = fullscreen_image[100:320, 1540:1810]
			d_crop = fullscreen_image[230:450, 1540:1810]

			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfoG",d_crop))
		elif wtype == 4: #melee
			r_crop = fullscreen_image[80:300, 1540:1810]
			a_crop = fullscreen_image[290:510, 1540:1810]

			crops.append(("MRankInfo",r_crop))
			crops.append(("AnimationSpeeds",a_crop))
		return crops

	#version 10.0.1
	def cropv1001(self, fullscreen_image, wtype, crops):
		if wtype == 1: #primary 
			#advanced menu crops: 0_0
			a_crop = fullscreen_image[400:630, 1290:1560]
			b_crop = fullscreen_image[150:380, 1290:1560]
			w_crop = fullscreen_image[555:790, 1290:1560]
			m_crop = fullscreen_image[700:920, 1290:1560]
			#main crops: 0_0
			r_crop = fullscreen_image[150:390, 1580:1850]
			d_crop = fullscreen_image[365:472, 1580:1850]
			f_crop = fullscreen_image[500:720, 1580:1850]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
		elif wtype == 2:
			#advanced menu crops: 0_0
			a_crop = fullscreen_image[400:630, 1290:1560]
			b_crop = fullscreen_image[150:380, 1290:1560]
			w_crop = fullscreen_image[555:790, 1290:1560]
			m_crop = fullscreen_image[700:920, 1290:1560]
			#main crops: 0_0
			r_crop = fullscreen_image[150:390, 1580:1850]
			d_crop = fullscreen_image[345:447, 1580:1850]
			f_crop = fullscreen_image[480:700, 1580:1850]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
			
		elif wtype == 3: #grenade
			r_crop = fullscreen_image[100:300, 1580:1850]
			d_crop = fullscreen_image[290:510, 1580:1850]
			da_crop = fullscreen_image[390:605, 1580:1850]

			crops.append(("GRankInfo",r_crop))
			crops.append(("DamageAdvanced",d_crop))
			crops.append(("DamageAdvanced2",da_crop))
		elif wtype == 4: #melee
			r_crop = fullscreen_image[150:365, 1580:1850]
			a_crop = fullscreen_image[220:440, 1580:1850]

			crops.append(("MRankInfo",r_crop))
			crops.append(("AnimationSpeeds",a_crop))
		return crops

	#version 10.1.0
	def cropv1010(self, fullscreen_image, wtype, crops):
		if wtype == 1: #gun
			#advanced menu crops:
			a_crop = fullscreen_image[275:510, 1290:1560] #300:515?
			b_crop = fullscreen_image[120:340, 1290:1560]
			w_crop = fullscreen_image[535:750, 1290:1560]
			m_crop = fullscreen_image[700:910, 1290:1560]
			#main crops
			r_crop = fullscreen_image[150:370, 1580:1850]
			d_crop = fullscreen_image[370:470, 1580:1850]
			f_crop = fullscreen_image[500:720, 1580:1850]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
		elif wtype == 2:
			a_crop = fullscreen_image[275:510, 1290:1560] #300:515?
			b_crop = fullscreen_image[120:340, 1290:1560]
			w_crop = fullscreen_image[535:750, 1290:1560]
			m_crop = fullscreen_image[700:910, 1290:1560]
			#main crops
			r_crop = fullscreen_image[150:370, 1580:1850]
			d_crop = fullscreen_image[370:470, 1580:1850]
			f_crop = fullscreen_image[500:720, 1580:1850]

			crops.append(("Ballistics", b_crop))
			crops.append(("Accuracies", a_crop))
			crops.append(("WeaponCharacteristics",w_crop))
			crops.append(("Miscellaneous",m_crop))
			crops.append(("RankInfo",r_crop))
			crops.append(("DamageInfo",d_crop))
			crops.append(("FireInfo",f_crop))
		elif wtype == 3: #grenade
			r_crop = fullscreen_image[100:300, 1580:1850]
			d_crop = fullscreen_image[300:395, 1580:1850]
			da_crop = fullscreen_image[390:605, 1580:1850]

			crops.append(("GRankInfo",r_crop))
			crops.append(("DamageAdvanced",d_crop))
			crops.append(("DamageAdvanced2",da_crop))
		elif wtype == 4: #melee
			r_crop = fullscreen_image[150:365, 1580:1850]
			a_crop = fullscreen_image[240:460, 1580:1850]

			crops.append(("MRankInfo",r_crop))
			crops.append(("AnimationSpeeds",a_crop))
		return crops

	# testing with big crop for advanced stats
	def cropv9999(self, fullscreen_image, wtype, crops):
		b_crop = fullscreen_image[130:900, 1290:1560]
		#c_crop = fullscreen_image
		crops.append(("bigcrop",b_crop))
		return crops
	# testing with afterglow font
	def cropv9998(self, fullscreen_image, wtype, crops):
		a_crop = fullscreen_image[240:460, 20:300]
		crops.append(("afterglow",a_crop))
		return crops


	def crop_fullscreen(self, fullscreen_image, wtype, version):
		fullscreen_image = testimagepath(fullscreen_image)
		crops = []
		print(version)
		if version == "902":
			crops = self.cropv902(fullscreen_image, wtype, crops)
		elif version == "903":
			crops = self.cropv902(fullscreen_image, wtype, crops)
		elif version == "1001":
			crops = self.cropv1001(fullscreen_image, wtype, crops)
		elif version == "1010":
			crops = self.cropv1010(fullscreen_image, wtype, crops)
		elif version == "9999":
			crops = self.cropv9999(fullscreen_image, wtype, crops)
		elif version == "9998":
			crops = self.cropv9998(fullscreen_image, wtype, crops)
		else:
			crops = crops

		#todo: add support for 800, 801, 802?
		#likely not needed since I am confident in the text files

		return crops
	
	
	def text_from_image(self, image, cropname, version):
		
		if type(image) is str:
			image = cv2.imread(image)
		if image.size == 0:
			return "[OCR] Empty Image" #uh oh
		bw		= cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
		resize	= bw

		
		if cropname != "bigcrop" and cropname != "DamageAdvanced" and cropname != "DamageInfo": #if this isn't bigcrop, scale it
			dim			= (int(bw.shape[0] * (self.resize_scale + 4)), int(bw.shape[1] * (self.resize_scale)))
			resize		= cv2.resize(bw, dim, interpolation=cv2.INTER_AREA)
		#elif cropname == "DamageAdvanced" or cropname == "DamageInfo":
			#dim			= (int(bw.shape[0] * (self.resize_scale+30)), int(bw.shape[1] * self.resize_scale))
			#resize		= cv2.resize(bw, dim, interpolation=cv2.INTER_AREA)
			##### PREPROCESSING

		#process = unsharp_mask(resize)
		process1		= cv2.cvtColor( resize, cv2.COLOR_GRAY2BGR)

		lab				= cv2.cvtColor(process1, cv2.COLOR_BGR2LAB)
		l_channel, a, b = cv2.split(lab)
		# Applying CLAHE to L-channel
		# feel free to try different values for the limit and grid size:
		clahe			= cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8,8))
		cl				= clahe.apply(l_channel)
		# merge the CLAHE enhanced L-channel with the a and b channel
		limg			= cv2.merge((cl,a,b))
		# Converting image from LAB Color model to BGR color spcae
		enhanced_img	= cv2.cvtColor(limg, cv2.COLOR_LAB2BGR)
		process2		= cv2.cvtColor(enhanced_img, cv2.COLOR_BGR2GRAY)
		sharpened_img	= cv2.bilateralFilter(process2,9,75,75)
		im_bw	= cv2.threshold(sharpened_img, 140, 255, cv2.THRESH_BINARY )[1]
		eroded			= thin_font(im_bw)
		filename = "pythoncvtesting/" + cropname + version + "_eroded.png"
		filename2 = "pythoncvtesting/" + cropname + version + "_sharpened.png"
		filename3 = "pythoncvtesting/" + cropname + version + "_monotone.png"
		cv2.imwrite(filename, eroded)
		cv2.imwrite(filename2,sharpened_img)
		cv2.imwrite(filename3, im_bw)
		
		rois = []
		if cropname == "DamageInfo" or cropname == "DamageAdvanced":
			
			cv2.imwrite("temp.png", im_bw)
			tempimage = cv2.imread("temp.png")#thin_font(enhanced_img)
		
			gray			= cv2.cvtColor(tempimage, cv2.COLOR_BGR2GRAY)
			blur			= cv2.GaussianBlur(gray, (7,7), 0)
			a = "pythoncvtesting/index_blur_" + cropname + version + ".png"
			cv2.imwrite(a, blur)
		
			threshold		= cv2.threshold(blur, 150, 255,  cv2.THRESH_OTSU)[1]
			a = "pythoncvtesting/index_threshold_" + cropname + version + ".png"
			cv2.imwrite(a, threshold)
		
			kernal = cv2.getStructuringElement(cv2.MORPH_RECT, (4,2)) #big version is around 56, 16
			a = "pythoncvtesting/index_kernal_" + cropname + version + ".png"
			cv2.imwrite(a, kernal)
		
			
			dilate = cv2.dilate(threshold, kernal, iterations=1)
			a = "pythoncvtesting/index_dilate_" + cropname + version + ".png"
			cv2.imwrite(a, dilate)

			counter  = 0
			cnts = cv2.findContours(dilate, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
			cnts = cnts[0] if len(cnts) == 2 else cnts[1]
			cnts = sorted(cnts, key=lambda x: cv2.boundingRect(x)[0])
			for c in cnts:
				x, y, w, h, = cv2.boundingRect(c)
				if w > 10 or h > 12:
					#threshold2 = cv2.threshold(sharpened_img, 120, 255, cv2.THRESH_BINARY_INV)[1]
					#cv2.imwrite("temp2.png", threshold2)
					#inp = cv2.imread("temp2.png")
					rois.append((counter,sharpened_img[y:y+h, x:x+w]))
					fi = "pythoncvtesting/index_roi_" + str(counter) + "_" + cropname + version + ".png"
					cv2.imwrite(fi, rois[counter][1])
					counter = counter + 1
					cv2.rectangle(tempimage, (x,y), (x+w,y+h), (36, 255, 12), 2)
			a = "pythoncvtesting/index_bbox_" + cropname + version + ".png"
			cv2.imwrite(a, tempimage)
			

			##### OCR

		if cropname == "GRankInfo" or cropname == "MRankInfo":
			return pytesseract.image_to_string(eroded, config="--psm 6") # (1 or 4) and 6 seem to work best atm
		elif cropname == "DamageInfo" or cropname == "DamageAdvanced":
			data = ""
			for roi in rois:
				data += ("index " + str(roi[0]) + ": ")
				data += pytesseract.image_to_string(roi[1], config="--psm 7")
			return data
		#elif cropname == "afterglow":
			#return pytesseract.image_to_string(bw, config="--psm 6")
		elif cropname == "FireInfo" :
			return pytesseract.image_to_string(sharpened_img, config="--psm 6")
		elif cropname == "Miscellaneous" or cropname == "bigcrop":
			return pytesseract.image_to_string(sharpened_img, config="--psm 6")

		data = pytesseract.image_to_string(sharpened_img, config="--psm 4")
		return data



#command arguments:
# 1: tessbin location
# 2: image location
# 3: type of weapon (1 = gun, 2 = grenade, 3 = melee)
tessbinfilepath = ""
#key images: 0_0 (AK12), 0_1 (AN94), 7_6 (E-GUN), 7_7 (AA12), 8_24 (ALIEN), 8_25 (AF2011-A1), 14_0 (RGN UDZS), 17_15 (CLONKER)

print("Current working directory is: " + os.getcwd())

if len(sys.argv) == 1:
	print("i force fed myself olives for a month, i hated it, but now i love olives")
else:
	if sys.argv[1] == "--help" or sys.argv[1] == "-h":
		print("""
Image Parser for Phantom Forces Database
COMMAND USAGE: impa [OPTIONS] {TESSBINPATH} FILEPATH TYPE VERSION ()
		
OPTIONS:
	-h  --help  Displays this help message, and exits
	-c          Tessbin folder exists in the current working directory (TESSBINPATH is ignored)
	-f          Tessbin folder exists in another directory (specified by TESSBINPATH)

	NOTE: -c and -f are mutually exclusive; you cannot use both at the same time.
		
TESSBINPATH: path to /tessbin/ (important for pytesseract). ignored if -c flag is specified

FILEPATH: path to file to be processed

TYPES: Specifies the type of weapon to be processed
	1 = Primary
	2 = Secondary
	3 = Grenade
	4 = Melee

VERSION: specifies the version of Phantom Forces

	Versions to be implemented: 800, 801, 802 
	Currently supported versions: 902, 903, 1001, 1010
	Experimental versions: 9999, 9998
		""")
		
		sys.exit()
	elif sys.argv[1].find("c") >= 0 and sys.argv[1].find("f") >= 0:
		print("-c and -f are mutually exclusive and cannot be used at the same time.")
		sys.exit()
	elif sys.argv[1].find("c") >= 0:
		if len(sys.argv) == 5:
			pa = ImageParser("")
			pa.parse_screenshot(sys.argv[2], int(sys.argv[3]), sys.argv[4])#tessbinpath becomes filepath, filepath becomes type, type becomes version
			if sys.argv[1].find("w") >=	0:
				categoryweaponcrop(sys.argv[2], int(sys.argv[3]), sys.argv[4])
		elif len(sys.argv) > 5:
			print("Too many parameters. Expected 4: imp -c FILEPATH TYPE VERSION")
		else:
			print("Too few parameters. Expected 4: imp -c FILEPATH TYPE VERSION")
			sys.exit()
	elif sys.argv[1].find("f") >= 0:
		if len(sys.argv) == 6:
			pa = ImageParser(sys.argv[2])
			pa.parse_screenshot(sys.argv[3], int(sys.argv[4]), sys.argv[5])
			if sys.argv[1].find("w") >=	0:
				categoryweaponcrop(sys.argv[3], int(sys.argv[4]), sys.argv[5])
		elif len(sys.argv) > 6:
			print("Too many parameters. Expected 5: imp -f TESSBINPATH FILEPATH TYPE VERSION")
		else:
			print("Too few parameters. Expected 5: imp -f TESSBINPATH FILEPATH TYPE VERSION")
			sys.exit()
	else:
		if sys.argv[1].find("w") >=	0:
			categoryweaponcrop(sys.argv[2], int(sys.argv[3]), sys.argv[4])
		print("i force fed myself olives for a month, i hated it, but now i love olives")



#pyinstaller command: py -m PyInstaller --onefile --paths=C:\Users\peter\AppData\Local\Programs\Python\Python312 imp.py
	

"i force fed myself olives for a month, i hated it, but now i love olives" #quote by dw harder


#pa = ImageParser()
#pa.parse_screenshot(sys.argv[3],int(sys.argv[4]))
#pa.parse_screenshot("C:\\Users\\Aethelhelm\\Programming\\source\\repos\\PFDB_API\\ImageParserForAPI\\13_2.png",2)
