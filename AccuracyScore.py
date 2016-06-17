from difflib import SequenceMatcher
def AccuracyScore(transcribed_text,audio_file):
    '''AccuracyScore accepts transcibed text string, and name of the audio file transcribed,
       Refernece transcribed file must be named as <audio_file>_transcribed '''
    try:
        transcribed_file = audio_file+"_transcribed.txt"
        file_obj = open(transcribed_file,'r')
        true_text = file_obj.read()
        print "What is actually said: " +true_text
        print "----------------------------------------------\n\n"
        print "Similarity of the two is: " +str(100*SequenceMatcher(None, true_text,transcribed_text).ratio())+"%"
        file_obj.close()
    except IOError:
        print audio_file+"_transcribed.txt file not present"
