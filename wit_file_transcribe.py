import speech_recognition as sr
import time
from AccuracyScore import AccuracyScore
#audio_file ="benchmark1"
#audio_file= "NATO_Phonetic_Alphabet_reading"
audio_file ="benchmark13"

r = sr.Recognizer()

with sr.AudioFile(audio_file+".wav") as source:
    audio = r.record(source)

WIT_AI_KEY = "DOSTUSERKZWEJE6BEYUBQ6W43NFP6LTJ"
try:
    start=time.clock()
    recog_text = r.recognize_wit(audio, key = WIT_AI_KEY)
    end = time.clock()
    print "wit thinks you said: " + recog_text
    print "----------------------------------------------\n\n"
    print "time taken: " + str((end-start)) +"s"
    
    print "----------------------------------------------\n\n"
    if recog_text:
        AccuracyScore(recog_text, audio_file)

except sr.UnknownValueError:
    print("Wit.ai could not understand audio")
except sr.RequestError as e:
    print("Could not request results from Wit.ai service; {0}".format(e))
