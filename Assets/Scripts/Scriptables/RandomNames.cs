using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "RandomName", menuName = "Scriptables/RandomName")]
    public class RandomNames : ScriptableObject
    {
        [SerializeField] private string[] mFirstNames;
        [SerializeField] private string[] fFirstNames;
        [SerializeField] private string[] lastNames;
        [SerializeField] private string[] nickNames;
        [SerializeField] private string[] titles;
        [SerializeField] private string[] titleDividersL;
        [SerializeField] private string[] titleDividersR;

        public string GetRandomName(bool male, bool includeName, bool includeLastName, bool includeNickname, bool includeTitle, 
            bool startsWithTitle, bool replaceNameWithNickname, bool lastNameThenName, bool useTitleDividers)
        {
            string fName = (male ? mFirstNames : fFirstNames).Length > 0 ? (male ? mFirstNames : fFirstNames)[Random.Range(0, (male ? mFirstNames : fFirstNames).Length)] : "Fernand@";
            string lName = (lastNames.Length > 0)? lastNames[Random.Range(0, lastNames.Length)] : "Escalé";
            string nName = (nickNames.Length > 0)? nickNames[Random.Range(0, nickNames.Length)] : "Ferch@";
            string title = (titles.Length > 0)? titles[Random.Range(0, titles.Length)] : "Enjoyer of Anime Girls";
            string titleDividerL = (titleDividersL.Length > 0)? titleDividersL[Random.Range(0, titleDividersL.Length)] : "-";
            string titleDividerR = (titleDividersR.Length > 0)? titleDividersR[Random.Range(0, titleDividersR.Length)] : "-";
            if (replaceNameWithNickname)
            {
                includeName = true;
                includeNickname = false;
                fName = nName;
            }
            string namePart = (includeName)? fName : "";
            string lastNamePart = (includeLastName)? lName : "";
            string nicknamePart = (includeNickname)? $"({nName})" : "";
            string titlePart = (includeTitle)? title : "";
            string fullName = "";
            
            if (useTitleDividers && includeTitle) titlePart = $"{titleDividerL}{title}{titleDividerR}";
            fullName = (lastNameThenName)? $"{lastNamePart} {namePart}".Trim() : $"{namePart} {lastNamePart}".Trim();
            fullName = (startsWithTitle)? $"{titlePart} {fullName}".Trim() : $"{fullName} {titlePart}".Trim();
            fullName = $"{fullName} {nicknamePart}".Trim();

            return (string.IsNullOrWhiteSpace(fullName))? null : fullName;
        }
    }
}
