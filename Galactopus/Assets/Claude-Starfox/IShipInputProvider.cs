using UnityEngine;

/// <summary>
/// Contrato que debe cumplir cualquier fuente de input para la nave.
/// Gracias a esta interfaz, el resto de los sistemas (movimiento, tilt,
/// disparo, polaridad) NUNCA saben si el input viene de teclado, mando,
/// una IA, o un replay grabado. Esto es lo que evita el "spaghetti":
/// si mañana quieres agregar soporte para un nuevo dispositivo o una IA
/// que controle una nave enemiga, solo creas OTRA clase que implemente
/// esta interfaz y la conectas, sin tocar ni una línea de los demás scripts.
/// </summary>
public interface IShipInputProvider
{
    Vector2 MoveInput { get; }
    bool FireHeld { get; }
    bool FirePressedThisFrame { get; }
    bool BoostHeld { get; }
    bool BrakeHeld { get; }
    bool BarrelRollLeftTriggered { get; }
    bool BarrelRollRightTriggered { get; }
    bool PolaritySwitchTriggered { get; }
}
