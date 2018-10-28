using System;
using UnityEngine.Events;

[Serializable]
public class DamageEvent : UnityEvent<DamageDealer, DamageTaker> { }

[Serializable]
public class NonDamageEvent : UnityEvent<DamageDealer> { }

[Serializable]
public class HealEvent : UnityEvent<float, DamageTaker> { }