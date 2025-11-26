using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Jailbreak.Public.Extensions;
using Jailbreak.Public.Mod.LastRequest;
using Jailbreak.Public.Mod.LastRequest.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace Jailbreak.LastRequest.LastRequests;

public abstract class TeleportingRequest(BasePlugin plugin,
  ILastRequestManager manager, CCSPlayerController prisoner,
  CCSPlayerController guard)
  : AbstractLastRequest(plugin, manager, prisoner, guard)
{
    public override void Setup()
    {
        State = LRState.PENDING;

        // Get guard pawn
        var guardPawn = Guard?.Pawn?.Value;
        var guardAng = guardPawn?.AbsRotation;
        if (guardPawn == null || guardAng == null)
            return;

        // Determine base position: prefer ground entity if available
        Vector basePos;
        var groundHandle = guardPawn.GroundEntity;
        if (groundHandle.IsValid || groundHandle.Value == null)
        {
            // No ground entity: use pawn origin
            basePos = guardPawn.AbsOrigin;
        }
        else
        {
            // On ground: use ground entity origin + small lift
            var groundEnt = groundHandle.Value;
            basePos = groundEnt.AbsOrigin + new Vector(0, 0, 1f);
        }

        // Convert angles to forward vector
        float pitchRad = guardAng.X * (float)(Math.PI / 180.0);
        float yawRad = guardAng.Y * (float)(Math.PI / 180.0);

        var forward = new Vector(
            (float)(Math.Cos(pitchRad) * Math.Cos(yawRad)),
            (float)(Math.Cos(pitchRad) * Math.Sin(yawRad)),
            0f // force horizontal direction
        );
        forward = Normalize(forward);

        const float distanceInFront = 130f;
        var targetPos = basePos + forward * distanceInFront;

        // Opposite facing angle for the prisoner
        var oppositeAngle = new QAngle(
            guardAng.X,
            guardAng.Y + 180f,
            guardAng.Z
        );

        // Teleport on next frame
        Server.NextFrame(() =>
        {
            Prisoner.Pawn?.Value?.Teleport(targetPos, oppositeAngle);

            Guard.Freeze();
            Prisoner.Freeze();

            Plugin.AddTimer(2, () => Guard.UnFreeze());
            Plugin.AddTimer(3, () => Prisoner.UnFreeze());
        });
    }

    /// <summary>Returns the Euclidean length of the vector.</summary>
    public static float Length(Vector vector)
    {
        return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
    }

    /// <summary>Returns a unit‐length vector pointing in the same direction.  
    /// If this vector is (near) zero, returns Vector(0,0,0).</summary>
    public static Vector Normalized(Vector vector)
    {
        float len = Length(vector);
        if (len <= 1e-6f)
            return new Vector(0f, 0f, 0f);
        return new Vector(vector.X / len, vector.Y / len, vector.Z / len);
    }

    /// <summary>In‐place normalization (modifies this instance).</summary>
    public static Vector Normalize(Vector vector)
    {
        float len = Length(vector);
        if (len > 1e-6f)
        {
            vector.X /= len;
            vector.Y /= len;
            vector.Z /= len;
        }
        else
        {
            vector.X = vector.Y = vector.Z = 0f;
        }

        return vector;
    }
}