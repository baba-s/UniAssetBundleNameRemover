using System.Linq;
using UnityEditor;

namespace Kogane.Internal
{
	internal static class AssetBundleNameRemover
	{
		private const string PACKAGE_NAME = "UniAssetBundleNameRemover";

		[MenuItem( "Edit/" + PACKAGE_NAME + "/アセットバンドル名をすべて削除" )]
		private static void Remove()
		{
			if ( !EditorUtility.DisplayDialog( PACKAGE_NAME, "アセットバンドル名をすべて削除しますか？", "はい", "いいえ" ) )
			{
				return;
			}

			AssetDatabase.StartAssetEditing();

			try
			{
				var targets = AssetDatabase
						.GetAllAssetPaths()
						.Select( x => AssetImporter.GetAtPath( x ) )
						.Where( x => !string.IsNullOrWhiteSpace( x.assetBundleName ) )
						.ToArray()
					;

				var count = targets.Length;

				for ( var i = 0; i < count; i++ )
				{
					var number        = i + 1;
					var progress      = ( float ) number / count;
					var assetImporter = targets[ i ];

					EditorUtility.DisplayProgressBar( PACKAGE_NAME, $"{number}/{count}", progress );

					assetImporter.SetAssetBundleNameAndVariant( null, null );
					assetImporter.SaveAndReimport();
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
				EditorUtility.ClearProgressBar();
			}

			foreach ( var n in AssetDatabase.GetAllAssetBundleNames() )
			{
				AssetDatabase.RemoveAssetBundleName( n, true );
			}

			AssetDatabase.SaveAssets();

			EditorUtility.DisplayDialog( PACKAGE_NAME, "アセットバンドル名をすべて削除しました", "OK" );
		}
	}
}