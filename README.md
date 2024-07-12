# LycansModTemplate

Ce repo contiens un mod d'exemple pour Lycans.

Vous pouvez cloner ce repo ou vous en servir comme d'un Template, ouvrir le fichier 
solution dans **Visual Studio** et commencer à développer directement.

Ce plugin d'exemple devrait vous permettre d'avoir tout ce dont vous avez besoin pour bien 
commencer à développer un mod simple. Il setup les éléments suivants :

- Un projet `.csproj` préconfigurer avec les paquets nécessaires
- Un logger global
- Un plugin basique qui broadcast un son

## Publier sur Thunderstore

Ce template propose aussi un exemple de package que vous pouvez utiliser pour uploader votre mod sur [Thunderstore](https://thunderstore.io/c/lycans/) :

- Placez vos .dll dans le dossier **Thunderstore\plugins\\**
- Modifiez le fichier `manifest.json` avec les informations de votre mod
- Écrivez un petit README afin de décrire ce que fait votre mod
- Modifiez l'image `icon.png` pour changer l'icone de votre mod (l'image doit être aux dimensions 256x256 **obligatoirement**)
- Rendez-vous enfin sur la page [d'upload de Thunderstore](https://thunderstore.io/c/lycans/create/)