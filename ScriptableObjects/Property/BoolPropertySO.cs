using UnityEngine;

[CreateAssetMenu(fileName = "_BoolPropertySO", menuName = "ScriptableObjects/Property/BoolPropertySO", order = 2)]
public class BoolPropertySO : PropertySO<bool>
{
  public override void ModifyValue( bool change )
  {
    SetValue( change );
  }
}