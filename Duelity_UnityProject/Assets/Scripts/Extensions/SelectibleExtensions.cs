using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public static class SelectibleExtensions
{
    public static void SetNavigationAccordingToPosition(this IEnumerable<Selectable> selectables)
    {
        if (selectables == null)
            return;

        foreach (var selectable in selectables)
        {
            RectTransform rect = selectable.GetComponent<RectTransform>();

            // Left
            if (TryGetClosestSelectable(rect,
                selectables.Select(s => s.GetComponent<RectTransform>()),
                (other) => other.position.x < rect.position.x,
                out var closestToLeft))
            {
                Navigation navigation = selectable.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnLeft = closestToLeft;
                selectable.navigation = navigation;
            }

            // Right
            if (TryGetClosestSelectable(rect,
                selectables.Select(s => s.GetComponent<RectTransform>()),
                (other) => other.position.x > rect.position.x,
                out var closestToRight))
            {
                Navigation navigation = selectable.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnRight = closestToRight;
                selectable.navigation = navigation;
            }

            // Up
            if (TryGetClosestSelectable(rect,
                selectables.Select(s => s.GetComponent<RectTransform>()),
                (other) => other.position.y > rect.position.y,
                out var closestToTop))
            {
                Navigation navigation = selectable.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnUp = closestToTop;
                selectable.navigation = navigation;
            }

            // Down
            if (TryGetClosestSelectable(rect,
                selectables.Select(s => s.GetComponent<RectTransform>()),
                (other) => other.position.y < rect.position.y,
                out var closestDown))
            {
                Navigation navigation = selectable.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnDown = closestDown;
                selectable.navigation = navigation;
            }
        }

        bool TryGetClosestSelectable(RectTransform selectable,
            IEnumerable<RectTransform> selectables,
            Func<RectTransform, bool> validator,
            out Selectable closestSelectable)
        {
            closestSelectable = selectables.Where(validator)
                .OrderBy(s => (s.anchoredPosition - selectable.anchoredPosition).sqrMagnitude)
                .FirstOrDefault()
                ?.GetComponent<Selectable>();
            return closestSelectable != null;
        }
    }
}
