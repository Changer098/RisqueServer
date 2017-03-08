from Crypto.Cipher import AES
import base64

'''

'''


class Cipher:

    iv = 0
    key = 0
    mode = AES.MODE_CBC
    aesObj = None
    zeroPadder = None

    def __init__(self, newIv, newKey):
        if len(iv) % 16 != 0 or len(iv) == 0:
            raise ValueError("IV is an incorrect length: " + len(iv))
        if len(key) % 16 != 0 or len(key) == 0:
            raise ValueError("Key is an incorrect length: " + len(key))
        self.iv = newIv
        self.key = newKey
        self.mode = AES.MODE_CBC
        self.zeroPadder = ZeroPadder()
        self.aesObj = AES.new(self.key, AES.MODE_CBC, self.iv)

    def encode(self, text):
        # Recreate the AES obj because python is stupid
        self.aesObj = AES.new(self.key, AES.MODE_CBC, self.iv)
        padText = self.zeroPadder.pad(text)         # Pad the text
        ciphText = self.aesObj.encrypt(padText)     # Encrypt to a bitstring (I think)
        return base64.b64encode(ciphText)           # Return a base64 encoded string

    def decode(self, text):
        # Recreate the AES obj because python is stupid
        self.aesObj = AES.new(self.key, AES.MODE_CBC, self.iv)
        plaincrypt = base64.b64decode(text)         # Convert from base64
        plaintext = self.aesObj.decrypt(plaincrypt) # Decrypt to base64
        return self.zeroPadder.depad(plaintext)     # Return unpadded plaintext


class ZeroPadder:

    @staticmethod
    def pad(text):
        array = bytearray(text)
        l = abs(16 - (len(text) % 16))
        for i in range(l):
            array.append(0)
        return array.decode("utf-8")

    @staticmethod
    def depad(text):
        array = bytearray(text)
        newArray = bytearray()
        for i in range(0, len(array), 1):
            # print "i: " + str(i) + ": " + str(array[i])
            if array[i] == 0:
                break;
            if array[i] == 5:
                # Surprise PKCS7 Padding
                break;
            newArray.append(array[i])
        return newArray.decode("utf-8")


#iv = "1234567890123456"
#key = "key1234567897891"
key = 'your key 16bytes'
iv = '1234567812345678'

cipher = Cipher(newIv=iv, newKey=key)

encoded = cipher.encode("hello world")
print "Encoded: " + encoded
roundtrip = cipher.decode(encoded)
print "Roundtrip: " + roundtrip

longtext = "Lorem ipsum dolor sit salem illa idem eam"
encoded = cipher.encode(longtext)
print "Encoded: " + encoded
roundtrip = cipher.decode(encoded)
print "Roundtrip: " + roundtrip

cipher = Cipher("1234567812345678", "your key 16bytes")
fromSharp = "IMb+e5BoVMY1Pb+I5O3HK2GAJhlHURr5mWDT81KUo/w="
print "fromSharp Decoded: " + cipher.decode(fromSharp)