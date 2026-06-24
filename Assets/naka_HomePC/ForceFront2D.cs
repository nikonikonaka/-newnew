using UnityEngine;

[DisallowMultipleComponent] // 同じスクリプトを複数アタッチするミスを防ぐ属性
public class ForceFront2D : MonoBehaviour
{
    // sortingOrder（ソート順）は数値が大きいほど手前に描画される。
    // 999999 のような極端に大きい値を使うことで、ほぼ全てのスプライトより前に出せる。
    [SerializeField] int sortingOrder = 999999;

    // SpriteRenderer コンポーネントを保持しておくための変数。
    // 毎回 GetComponent を呼ぶと処理が重くなるため、Awake で一度だけ取得してキャッシュする。
    SpriteRenderer sr;

    void Awake()
    {
        // ▼ GetComponent<T>() の「T 型」とは？
        // T は「型（Type）」を表すジェネリック引数。
        // 例：GetComponent<SpriteRenderer>() の場合、
        //     T = SpriteRenderer という型が入る。
        //
        // ▼ コンポーネントとは？
        // Unity の GameObject に機能を追加する“部品”のこと。
        // Transform / SpriteRenderer / Rigidbody2D などがコンポーネント。
        //
        // ▼ GetComponent の意味：
        // この GameObject にアタッチされているコンポーネントの中から、
        // 指定した型（T 型）のものを探して返す関数。
        //アタッチは日本語で「付ける」「添付する」「結びつける」の意味
        // ▼ この行の意味：
        // このオブジェクトに付いている SpriteRenderer コンポーネントを取得する。
        sr = GetComponent<SpriteRenderer>();

        // このスクリプトは 2D スプライト専用なので、
        // SpriteRenderer が無い場合は警告を出して開発者に気づかせる。
        if (sr == null)
        {
            Debug.LogWarning($"{name}: SpriteRenderer がありません。ForceFront2D は 2D スプライト専用です。");
        }
    }

    void LateUpdate()
    {
        // SpriteRenderer が存在する場合のみ処理を行う。
        if (sr != null)
        {
            // sortingOrder を毎フレーム上書きする。
            //
            // ▼ なぜ LateUpdate() なのか？
            // Unity の実行順序では、
            //   Update() → LateUpdate()
            // の順に呼ばれるため、
            // 他のスクリプトが Update() で sortingOrder を変更しても、
            // 最後にこのスクリプトが上書きすることで「絶対最前面」を維持できる。
            sr.sortingOrder = sortingOrder;
        }
    }
}
