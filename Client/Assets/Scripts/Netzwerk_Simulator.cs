using UnityEngine;
using System.Collections;

/*
 * Das ist eine Testklasse um Netzwerkübertragungen zu simulieren. Für's Testen eine Art "peer to peer".
 * Alles kann über die "Senden"-Funktion gesendet werden und wird von jedem empfangen
 * der sich an dem event "NetzwerkStream" angemeldet hat.
 * Der letzte Parameter enthält Informationen (String in diesem Fall) und die ersten drei sind zur
 * erleichterten Bedienung.
 */

public enum PackageType
{
	Position, Rotation, Crouch, Demage, Granade, Velocity, pickedUp
};

public class Netzwerk_Simulator
{
	public static bool aktiv = true;
	public delegate void Del_NetzwerkStream(int sender, int empfaenger, PackageType typ, string info);
	public static event Del_NetzwerkStream NetzwerkStream;

	public static void Senden(int sender, int empfaenger, PackageType typ, string info)
	{
		if (NetzwerkStream != null && aktiv) NetzwerkStream(sender, empfaenger, typ, info);
	}
}