#pragma warning disable IDE0063 // Use simple 'using' statement

using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace JsonReaderLibrary
{
   public static class JsonReader
   {
      #region - Fields & Properties
      /// <summary>
      /// Settings used for handling type serialization.
      /// </summary>
      public static JsonSerializerSettings SerializeSettings { get; set; } = new JsonSerializerSettings
      {
         TypeNameHandling = TypeNameHandling.Objects,
         TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
      };
      /// <summary>
      /// Settings used for handling type deserialization.
      /// </summary>
      public static JsonSerializerSettings DeserializerSettings { get; set; } = new JsonSerializerSettings
      {
         TypeNameHandling = TypeNameHandling.Objects,
      };
      #endregion

      #region - Methods
      /// <summary>
      /// Saves <typeparamref name="TModel"/> to the provided path.
      /// </summary>
      /// <typeparam name="TModel">The type of object, usually infered from the data property.</typeparam>
      /// <param name="path">The path to the file.</param>
      /// <param name="data">The data to save.</param>
      /// <param name="createNew">Will create a new file if <see langword="true"/>.</param>
      /// <param name="autoIndent">Indents the saved JSON if <see langword="true"/>.</param>
      public static void SaveJsonFile<TModel>(
         string path,
         TModel data,
         bool createNew = true,
         bool autoIndent = true
      ) where TModel : new()
      {
         try
         {
            if (string.IsNullOrEmpty(path))
            {
               throw new ArgumentException($"Path cannot be empty: '{path}'");
            }

            if (!File.Exists(path) && !createNew)
            {
               throw new ArgumentException($"Path is not a file: '{path}'");
            }

            if (data == null)
            {
               throw new ArgumentException($"Provided data cannot be null.");
            }

            if (createNew)
            {
               using (StreamWriter writer = new StreamWriter(path))
               {
                  string json = JsonConvert.SerializeObject(data, autoIndent ? Formatting.Indented : Formatting.None);
                  writer.Write(json);
                  writer.Flush();
               }
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Saves the JSON file with explicit type declarations.
      /// <para/>
      /// Used to solve a problem with storing dictionaries.
      /// Use <see cref="SaveJsonFile{TModel}(string, TModel, bool, bool)"/> for normal data.
      /// </summary>
      /// <typeparam name="TModel">The type of object, usually infered from the data property.</typeparam>
      /// <param name="path">The path to the file.</param>
      /// <param name="data">The data to save.</param>
      /// <param name="createNew">Will create a new file if <see langword="true"/>.</param>
      /// <param name="autoIndent">Indents the saved JSON if <see langword="true"/>.</param>
      public static void SaveJsonFileExpTypes<TModel>(
         string path,
         TModel data,
         bool createNew = true,
         bool autoIndent = true
      ) where TModel : new()
      {
         try
         {
            if (string.IsNullOrEmpty(path))
            {
               throw new ArgumentException($"Path cannot be empty: '{path}'");
            }

            if (!File.Exists(path) && !createNew)
            {
               throw new ArgumentException($"Path is not a file: '{path}'");
            }

            if (data == null)
            {
               throw new ArgumentException($"Provided data cannot be null.");
            }

            if (createNew)
            {
               SerializeSettings.Formatting = autoIndent ? Formatting.Indented : Formatting.None;
               using (var writer = new StreamWriter(path))
               {
                  string json = JsonConvert.SerializeObject(data, SerializeSettings);
                  writer.Write(json);
                  writer.Flush();
               }
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Saves a tree data structure to a JSON file.
      /// <para/>
      /// Use only when needed. Not ment for normal files. Use <see cref="SaveJsonFile{TModel}(string, TModel, bool, bool)"/> instead.
      /// </summary>
      /// <typeparam name="TModel">The type of object, usually infered from the data property.</typeparam>
      /// <param name="path">The path to the file.</param>
      /// <param name="data">The data to save.</param>
      /// <param name="createNew">Will create a new file if <see langword="true"/>.</param>
      /// <param name="autoIndent">Indents the saved JSON if <see langword="true"/>.</param>
      public static void SaveJsonFileTree<TModel>(string path, TModel data, bool createNew = true) where TModel : new()
      {
         try
         {
            if (string.IsNullOrEmpty(path))
            {
               throw new ArgumentException($"Path cannot be empty: '{path}'");
            }

            if (!File.Exists(path) && !createNew)
            {
               throw new ArgumentException($"Path is not a file: '{path}'");
            }

            if (data == null)
            {
               throw new ArgumentException($"Provided data cannot be null.");
            }

            if (createNew)
            {
               var serializeSettings = new JsonSerializerSettings
               {
                  ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                  Formatting = Formatting.Indented,
               };
               using (StreamWriter writer = new StreamWriter(path))
               {
                  string json = JsonConvert.SerializeObject(data, serializeSettings);
                  writer.Write(json);
                  writer.Flush();
               }
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Saves the object to the provided path asynchronously.
      /// </summary>
      /// <typeparam name="TModel">The type of object, infered from the data property.</typeparam>
      /// <param name="path">The path to the file.</param>
      /// <param name="data">The data to save.</param>
      /// <param name="createNew">Will create a new file if true.</param>
      /// <param name="autoFormat">Saves indented.</param>
      /// <returns>A <see cref="Task"/> operation</returns>
      public static async Task SaveJsonFileAsync<TModel>(
         string path,
         TModel data,
         bool createNew = true,
         bool autoFormat = true
      ) where TModel : new()
      {
         try
         {
            await Task.Run(() => SaveJsonFile(path, data, createNew, autoFormat));
         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Opens the file and parses it with the provided type.
      /// </summary>
      /// <typeparam name="TModel">The type to convert the Json to.</typeparam>
      /// <param name="path">The path to the file.</param>
      /// <param name="useTypeNameHandle">Only <see langword="true"/> when <see cref="SaveJsonFileExpTypes{TModel}(string, TModel, bool, bool)"/> is needed.</param>
      /// <returns>The object as the <typeparamref name="TModel"/></returns>
      public static TModel OpenJsonFile<TModel>(string path, bool useTypeNameHandle = false) where TModel : new()
      {
         try
         {
            if (string.IsNullOrEmpty(path))
            {
               throw new ArgumentException($"Path cannot be empty: '{path}'");
            }

            if (!File.Exists(path))
            {
               throw new ArgumentException($"Path is not a file: '{path}'");
            }
            using (StreamReader reader = new StreamReader(path))
            {
               return useTypeNameHandle
                   ? JsonConvert.DeserializeObject<TModel>(reader.ReadToEnd(), DeserializerSettings)
                   : JsonConvert.DeserializeObject<TModel>(reader.ReadToEnd());
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Opens a file async asynchronously.
      /// </summary>
      /// <typeparam name="TModel">The type to convert the Json to.</typeparam>
      /// <param name="path">The path to the file.</param>
      /// <returns>The newly created <typeparamref name="TModel"/>.</returns>
      public static async Task<TModel> OpenJsonFileAsync<TModel>(string path) where TModel : new()
      {
         try
         {
            return await Task.Run(() => OpenJsonFile<TModel>(path));
         }
         catch (Exception)
         {
            throw;
         }
      }
      #endregion
   }
}
