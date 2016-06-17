# PESU-AerX-ATC_Sim
Repository of code and other files used to setup and evaluate Speech-to-Text Engines (Sphinx4, Wit.ai through Python Speech Recognition, etc.) 
-----------------------------------------------------------------------------------------------------------------------------------------
AccuracyScore.py:     
  Contains AccuracyScore(transcribed_text, audio_file),
  first argument accepts string(text transcribed by STT Engine), 
  second argument is name of the audio file being transcribed.
  
  Text file named '<audio_file>_transcribed' containing the true transcribed text must be present in same directory.
