from Crypto.Cipher import AES
import base64

'''

'''

class Cipher():
    iv = 0
    key = 0
    mode = AES.MODE_CBC
    def __init__(self, iv, key, mode):
        self.iv = iv
        self.key = key
        self.mode = mode
    def encode(self, text):
        return text
    def decode(self, text):
        return text

class ZeroPadder():
    @staticmethod
    def pad(self, text):
        array = bytearray(text)
        l = abs(16 - (len(text) % 16))
        for i in range(l):
            array.append(0)
        return array.decode("utf-8")
    @staticmethod
    def depad(self, text):
        array = bytearray(text)
        newArray = bytearray()
        for i in range(0, len(array), 1):
            #print "i: " + str(i) + ": " + str(array[i])
            if array[i] == 0:
                break;
            if array[i] == 5:
                #Surprise PKCS7 Padding
                break;
            newArray.append(array[i])
        return newArray.decode("utf-8")