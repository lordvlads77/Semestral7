
gource --title "Blade Bound github" --hash-seed 101 -a 1 --highlight-users --font-size 22 -e 0.1  --hide filenames,dirnames --seconds-per-day 0.10 --auto-skip-seconds 1 -1280x720 -o - | ffmpeg -y -r 30 -f image2pipe -vcodec ppm -i - -vcodec libx264 -preset ultrafast -pix_fmt yuv420p -crf 26 -threads 0 -bf 0 gource.mp4
PAUSE
