./pc.py archive create -f picross.zip
find -name "*.txt" | xargs -I{} ./pc.py archive add-from-solution picross.zip internet {}
./pc.py archive add-player picross.zip player