from cipher import Cipher

#iv = "1234567890123456"
#key = "key1234567897891"
key = 'risqueXXXXXXXXXX'
iv = 'ThisisanIV456XXX'

cipher = Cipher(iv, key)

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