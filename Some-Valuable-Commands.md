### How to prune everything in Docker?
```
docker system prune --all
```
### How to reboot WSL2 in Windows 10/11?
```
wsl --shutdown
wsl 
```
### How to shrink a virtual hard disk file (vhdx) of WSL2 in Windows 10/11?
```
diskpart
select vhdx file=C:\Users\U1\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu20.04onWindows_79rhkp1fndgsc\LocalState\ext4.vhdx
compact vhdx
```
### How to login to Docker Hub by command line?
```
sudo docker login -u [user-name] --password-stdin [password]
```
### How to tag and push an image to Docker Hub?
```
sudo docker tag [image id] [account or namespace]/azure-pipelines-agents-debian-10.13:11072023
sudo docker push [account or namespace]/azure-pipelines-agents-debian-10.13:11072023
```