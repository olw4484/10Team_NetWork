using System.Collections;

using UnityEngine;

// 중복 스크립트 방지
[DisallowMultipleComponent]
public class YSJ_SpawnObjectLifeCycle : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float _lifeTime = 3f;
    public bool _onceFirst = false;

    private Coroutine _lifeCoroutine;

    private bool _isPoolableObject = false;

    private void Awake()
    {
        // 플매니저가 업는지
        // 플매니저에 등록되었는지 여부
        // 플링 가능 한 옵젝 인지
        if (YSJ_PoolManager.Instance == null
            || !YSJ_PoolManager.Instance.HasPool(this.gameObject.name)
            || this.GetComponentInParent<YSJ_IPoolable>() == null)
        {
            return;
        }

        _isPoolableObject = true;
    }

    private void OnEnable()
    {
        // 재활성화 될 때마다 타이머 리셋
        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);

        _lifeCoroutine = StartCoroutine(LifeTimer());
    }

    private void OnDisable()
    {
        if (_onceFirst)
        {
            if (_lifeCoroutine != null)
                StopCoroutine(_lifeCoroutine);
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(_lifeTime);

        if (_isPoolableObject)
            YSJ_PoolManager.Instance.Despawn(gameObject);
        else
            Destroy(gameObject);
    }
    public void SetLifeTime(float time)
    {
        _lifeTime = time;

        // 즉시 적용
        if (isActiveAndEnabled)
        {
            if (_lifeCoroutine != null)
                StopCoroutine(_lifeCoroutine);

            _lifeCoroutine = StartCoroutine(LifeTimer());
        }
    }
}
