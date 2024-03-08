using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TMProTools
{
    private const string PREFAB_FILES_SEARCH_PATTERN = "*.prefab";
    private const string ASSET_FILES_SEARCH_PATTERN = "*.asset";
    private const string PACKAGE_FOLDER = "Packages";
    private const string PACKAGE_CACHE_FOLDER = "PackageCache";
    private const string TMPRO_COMPONENT_CAPTION = "textMeshProComponent";
    private const string MATERIAL_HEADER = "Material:";
    private const string TEXTURE2D_HEADER = "Texture2D:";
    private const string GLYPHTABLE_HEADER = "m_GlyphTable:";
    private const string SHADER_HEADER = "m_Shader:";

    private static HashSet<string> prefabsWithTMProComponentPaths = new();
    private static HashSet<string> assetsWithMaterialTexture2DAndGlyphsPaths = new();
    private static HashSet<string> shadersUsedByMaterialsPaths = new();
    private static HashSet<string> errosFoundDuringProcessing = new();

    private static DirectoryInfo repoRootDirectory = GetRepoRootDirectory();

    [MenuItem("MRTK3TMProTools/Analyze local repo")]
    private static void AnalyzeLocalRepo()
    {
        IEnumerable<FileInfo> processablePrefabs = GetNonPackageFilesBySearchPattern(repoRootDirectory, PREFAB_FILES_SEARCH_PATTERN);
        ProcessPrefabs(processablePrefabs);

        IEnumerable<FileInfo> processableAssets = GetNonPackageFilesBySearchPattern(repoRootDirectory, ASSET_FILES_SEARCH_PATTERN);
        ProcessAssets(processableAssets);

        IEnumerable<FileInfo> shadersUsedByAssetswMaterialAndTexture2D = GetShadersUsedByMaterials();
        ProcessShaders(shadersUsedByAssetswMaterialAndTexture2D);

        GenerateAnalysisReport();
    }

    private static void GenerateAnalysisReport()
    {
        //Summary header
        string report = $"Prefabs with TextMeshPro component: {prefabsWithTMProComponentPaths.Count}\n" +
                        $"Assets with Material, Texture2D, and GlyphTable: {assetsWithMaterialTexture2DAndGlyphsPaths.Count}\n" +
                        $"Shaders used by materials: {shadersUsedByMaterialsPaths.Count}\n" +
                        $"Errors found during processing: {errosFoundDuringProcessing.Count}\n\n";

        //Details
        report += $"* Prefabs ({prefabsWithTMProComponentPaths.Count}) with TextMeshPro component:\n";
        foreach (string prefabPath in prefabsWithTMProComponentPaths)
        {
            report += "\t<a href=\"" + prefabPath + $"\" line=\"2\">{prefabPath.Substring(prefabPath.IndexOf(repoRootDirectory.FullName) + repoRootDirectory.FullName.Length)}</a>\n";
        }
        report += "\n";

        report += $"* Assets ({assetsWithMaterialTexture2DAndGlyphsPaths.Count}) with Material, Texture2D, and GlyphTable:\n";
        foreach (string assetPath in assetsWithMaterialTexture2DAndGlyphsPaths)
        {
            report += "\t<a href=\"" + assetPath + $"\" line=\"2\">{assetPath.Substring(assetPath.IndexOf(repoRootDirectory.FullName) + repoRootDirectory.FullName.Length)}</a>\n";
        }
        report += "\n";

        report += $"* Shaders ({shadersUsedByMaterialsPaths.Count}) used by previously listed materials:\n";
        foreach (string shaderPath in shadersUsedByMaterialsPaths)
        {
            report += "\t<a href=\"" + shaderPath + $"\" line=\"2\">{shaderPath.Substring(shaderPath.IndexOf(repoRootDirectory.FullName) + repoRootDirectory.FullName.Length)}</a>\n";
        }
        report += "\n";

        report += $"* Errors ({errosFoundDuringProcessing.Count}) encountered during analysis:\n";
        foreach (string error in errosFoundDuringProcessing)
        {
            report += "\t" + error + "\n";
        }

        Debug.Log(report);
    }

    private static IEnumerable<FileInfo> GetShadersUsedByMaterials()
    {
        HashSet<FileInfo> result = new();

        foreach (string assetFile in assetsWithMaterialTexture2DAndGlyphsPaths)
        {
            File.ReadLines(assetFile)
                .Where(line => line.Contains(SHADER_HEADER))
                .ToList()
                .ForEach(line => {
                    FileInfo shaderPath = GetFileForGUIDInLine(line);
                    if (shaderPath != null)
                    {
                        result.Add(shaderPath);
                    }
                    else
                    {
                        errosFoundDuringProcessing.Add($"Shader file for GUID referenced in '{line}' of material {assetFile} not found");
                    }
                });
        }

        return result;
    }

    private static FileInfo GetFileForGUIDInLine(string line)
    {
        string shaderGUID = line.Substring(line.IndexOf("guid: ") + 6, 32);
        string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGUID);
        if (File.Exists(shaderPath))
        {
            return new FileInfo(shaderPath);
        }
        else
        {
            Debug.LogError($"File for GUID {shaderGUID} not found.");
            return null;
        }
    }

    private static void ProcessShaders(IEnumerable<FileInfo> processableShaders)
    {
        foreach (FileInfo shaderFile in processableShaders)
        {
            Debug.Log($"Shader {shaderFile.Name} is used by materials.");
            shadersUsedByMaterialsPaths.Add(shaderFile.FullName);
        }
    }

    private static void ProcessAssets(IEnumerable<FileInfo> processableAssets)
    {
        foreach (FileInfo assetFile in processableAssets)
        {
            bool hasMaterial = false;
            bool hasTexture2D = false;
            bool hasGlyphTable = false;

            File.ReadLines(assetFile.FullName)
                .Where(line => line.Contains(MATERIAL_HEADER) || line.Contains(TEXTURE2D_HEADER) || line.Contains(GLYPHTABLE_HEADER))
                .ToList()
                .ForEach(line =>
                {
                    if (line.Contains(MATERIAL_HEADER))
                    {
                        hasMaterial = true;
                    }
                    if (line.Contains(TEXTURE2D_HEADER))
                    {
                        hasTexture2D = true;
                    }
                    if (line.Contains(GLYPHTABLE_HEADER))
                    {
                        hasGlyphTable = true;
                    }
                });

            if (hasMaterial && hasTexture2D && hasGlyphTable)
            {
                Debug.Log($"Asset {assetFile.Name} contains Material, Texture2D, and GlyphTable.");
                assetsWithMaterialTexture2DAndGlyphsPaths.Add(assetFile.FullName);
            }
        }
    }

    private static void ProcessPrefabs(IEnumerable<FileInfo> processablePrefabs)
    {
        foreach (FileInfo prefabFile in processablePrefabs)
        {
            File.ReadLines(prefabFile.FullName)
                .Where(line => line.Contains(TMPRO_COMPONENT_CAPTION))
                .ToList()
                .ForEach(line =>
                {
                    Debug.Log($"Prefab {prefabFile.Name} contains TextMeshPro component: {line}");
                    prefabsWithTMProComponentPaths.Add(prefabFile.FullName);
                });
        }
    }

    private static IEnumerable<FileInfo> GetNonPackageFilesBySearchPattern(DirectoryInfo repoRootDirectory, string searchPattern)
    {
        return from prefabFile in repoRootDirectory.EnumerateFiles(searchPattern, SearchOption.AllDirectories)
               where !prefabFile.DirectoryName.Contains(PACKAGE_CACHE_FOLDER) &&
                     !prefabFile.DirectoryName.Contains(PACKAGE_FOLDER)
               select prefabFile;
    }

    private static DirectoryInfo GetRepoRootDirectory()
    {
        string repoRootDirectory = Path.Combine(Application.dataPath, "..", "..", "..");
        return new DirectoryInfo(Path.GetFullPath(repoRootDirectory));
    }
}
