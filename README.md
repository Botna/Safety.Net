# Safety.net




## Recombining Google drive files

in order to upload things succesfully to google drive, we upload your currently recording video files in chunk format.  To recombine open cmd and do the following

```
> copy /b vid.mp4_0 + vid.mp4_1 + vid.mp4_2 vid.mp4
```

This will build you a video file named `vid.mp4` that has everythign it needs, minus some metadata at the beginning of the file.

the file appende _headerData needs to replace the initial 16kb of the combined file you made above.  After that, the video is capable of being played.