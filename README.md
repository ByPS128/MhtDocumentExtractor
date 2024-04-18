# Web page mht document extractor

## Motivation for implementation:
I needed to look at backups of *.mht files I had saved once (circa 2010) for later use.  
I liked the idea of being able to save a page as one file when it consists of many images, style sheets, scripts, etc.
However, I found that I can no longer view such files in Google Chrome or Microsoft Edge.  

I searched for a while for extensions to browsers and in vain.  
I searched for online or desktop applications that could do this, but in vain.  

Everything I found worked with MIME formatted archives, but everything was focused on email conversations and their attachments.
None of the several apps I tried could do anything with my files.  

And that's the motivation.  

## What the app does:
You simply give it the path to the MHT file and the directory where the content will be extracted.  
The application will go through the document and extract all the files it finds into the output directory.  

Some files don't have a physical filename because their source was somewhere on the internet, so sometimes they are URL links from which the filename cannot be inferred.   
In that case, I generate an alternate name to base it on the file system and try to derive the extension from the MIME Type.  
The application modifies the links to the created files in the contents of all the files it has created, so that displaying and linking the files to each other works correctly.  

And that's all it does. You can then open and view the files that were created yourself.  

## Possible errors:
I didn't want to spend a lot of time developing the application, so I didn't cover and treat all possible error scenarios.  
I edited the application until it correctly unpacked the archive documents I needed.  
Therefore, it is possible that your file or multiple files might crash during extraction. Maybe on not implemented encoding.  

In that case, file an issue and let's take a look. If needed and within my power I will improve the application.  
            
The application works with documents in memory, so if your file is too big, it is possible that the application will need more memory.  

## Conclusion:
Hopefully the app will help you look into your archives.  
Good luck.
