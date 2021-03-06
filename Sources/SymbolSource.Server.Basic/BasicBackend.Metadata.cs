﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        private string GetPathToImageFile(string name, string symbolHash)
        {
            string binaryIndexPath = Path.Combine(configuration.IndexPath, name);
            if (!Directory.Exists(binaryIndexPath))
                return null;

            string hashIndexPath = Path.Combine(binaryIndexPath, symbolHash + ".txt");
            if (!File.Exists(hashIndexPath))
                return null;

            var hashes = File.ReadAllLines(hashIndexPath)
                .Where(h => Directory.Exists(Path.Combine(configuration.DataPath, h)))
                .ToArray();

            File.WriteAllLines(hashIndexPath, hashes);

            return hashes.FirstOrDefault();
        }

        private string GetPackagePathFromVersion(Version version, string packageFormat)
        {
            // ReSharper disable ReplaceWithSingleCallToFirstOrDefault
            return GetPackageNameCandidates(version, packageFormat)
                .Select(name => Path.Combine(version.Project, version.Name, name))
                .Where(candidate => File.Exists(Path.Combine(configuration.DataPath, candidate)))
                .FirstOrDefault();
            // ReSharper restore ReplaceWithSingleCallToFirstOrDefault
        }

        private IEnumerable<string> GetPackageNameCandidates(Version version, string packageFormat)
        {
            if (packageFormat != null)
                return new[] { GetPackageName(packageFormat, version.Project, version.Name) };

            return new[]
                       {
                           GetPackageName("NuGet", version.Project, version.Name),
                           GetPackageName("OpenWrap", version.Project, version.Name)
                       };
        }

        private string GetPathFromImageFile(ImageFile imageFile)
        {
            return Path.Combine(imageFile.Project, imageFile.Version, "Binaries", imageFile.Name, imageFile.SymbolHash);
        }

        private string GetPathFromSourceFile(SourceFile sourceFile)
        {
            return Path.Combine(sourceFile.Project, sourceFile.Version, "Sources", sourceFile.Path);
        }

        private ImageFile BuildImageFile(string path)
        {
            string[] parts = path.Split(Path.DirectorySeparatorChar);

            return new ImageFile
                       {
                           Repository = "Basic",
                           Project = parts[0],
                           Version = parts[1],
                           Platform = "Basic",
                           Mode = "Basic",
                           Name = parts[3],
                           SymbolHash = parts[4]
                       };
        }

        public ImageFile GetImageFile(string name, string symbolHash)
        {
            string pathImageFile = GetPathToImageFile(name, symbolHash);
            if (pathImageFile == null)
                return null;

            return BuildImageFile(pathImageFile);
        }

        public SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            var imageFileCopy = imageFile;

            string imagePath = GetPathFromImageFile(imageFile);
            string path = Path.Combine(configuration.DataPath, imagePath, imageFile.Name + ".txt");
            var sources = File.ReadAllLines(path)
                .Select(s => s.Split('|'))
                .Select(s => new SourceFile
                                 {
                                     Repository = imageFileCopy.Repository,
                                     Project = imageFileCopy.Project,
                                     Version = imageFileCopy.Version,
                                     Mode = imageFileCopy.Mode,
                                     Platform = imageFileCopy.Platform,
                                     ImageName = imageFileCopy.Name,
                                     Hash = "Basic",
                                     OriginalPath = s[0],
                                     Path = s[1],
                                 }
                )
                .ToArray();

            return sources;
        }

        public void CreateVersion(Version version)
        {
        }

        public Compilation[] GetCompilationList(ref Version version)
        {
            throw new NotImplementedException();
        }

        public void SetVersionHidden(ref Version version, bool hidden)
        {
            throw new NotImplementedException();
        }

        public Version[] GetVersions(ref Project project)
        {
            throw new NotImplementedException();
        }

        public void SetProjectPermissions(User targetUser, Project project, Permission permission)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void CreateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateRepository(Repository repository)
        {
            throw new NotImplementedException();
        }

        public Version[] GetPackages(ref Repository repository, string packageFormat)
        {
            var repositoryCopy = repository;

            return Directory.EnumerateDirectories(configuration.DataPath)
                .SelectMany(
                    projectPath =>
                    Directory.EnumerateDirectories(projectPath)
                        .Select(
                            versionPath =>
                            new Version
                                {
                                    Company = repositoryCopy.Company,
                                    Repository = repositoryCopy.Name,
                                    Project = Path.GetFileName(projectPath),
                                    Name = Path.GetFileName(versionPath),
                                    PackageFormat = packageFormat
                                })
                        .Where(version => GetPackagePathFromVersion(version, packageFormat) != null))
                .ToArray();
        }

        public Caller CreateUserByKey(string company, string type, string value)
        {
            throw new NotImplementedException();
        }
    }
}