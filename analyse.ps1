# docker run --rm -it -v C:/Users/dan/projects/das-employerapprenticeshipsservice:c:/projects/das-employerapprenticeshipsservice 3d9151a444b2

SonarScanner.MSBuild.exe begin /k:"SkillsFundingAgency_das-employerapprenticeshipsservice" /o:"educationandskillsfundingagency"
Nuget Restore c:\projects\das-employerapprenticeshipsservice\src\SFA.DAS.EAS.sln 
MSBuild.exe c:\projects\das-employerapprenticeshipsservice\src\SFA.DAS.EAS.sln /t:Rebuild
SonarScanner.MSBuild.exe end 