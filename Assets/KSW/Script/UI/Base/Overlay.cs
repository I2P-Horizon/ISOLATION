using UnityEngine;

public abstract class Overlay : MonoBehaviour
{
    [SerializeField] protected GameObject _background;
    protected UIAnimator _animator;

    protected virtual void Show()
    {
        _background.GetComponent<UIAnimator>().Show();
        _animator.Show();
    }

    protected virtual void Close()
    {
        _background.GetComponent<UIAnimator>().Close();
        _animator.Close();
    }

    protected virtual void Update()
    {
        if (Loading.Instance.isLoading) return;
    }

    protected virtual void Awake()
    {
        _animator = GetComponent<UIAnimator>();
    }
}