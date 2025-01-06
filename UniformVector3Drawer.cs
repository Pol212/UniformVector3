using UnityEngine;
using UnityEditor;
using System.Xml.Linq;

[CustomPropertyDrawer(typeof(UniformVector3Attribute))]
public class UniformVector3Drawer : PropertyDrawer {
    bool toggle = false;
    bool scaleGot = false;
    Vector3 scale = Vector3.one;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Verifica che il campo sia di tipo Vector3
        if (property.propertyType != SerializedPropertyType.Vector3) {
            EditorGUI.LabelField(position, label.text, "Use [UniformVector3] with Vector3.");
            return;
        }

        // Larghezza per l'etichetta principale (es. "Position")
        float labelWidth = EditorGUIUtility.labelWidth;

        // Disegna l'etichetta principale
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Larghezza complessiva per i campi numerici (lascia spazio per le etichette X, Y, Z)
        float totalFieldWidth = position.width;
        float left = position.x;
        float fieldWidth = 128f; // Campo per il numero meno spazio per le etichette
        float labelSpacing = 30f; // Spazio per le etichette X, Y, Z

        // Rettangoli per X, Y, Z e relative etichette
        Rect toggleRect = new Rect(position.x - 35, position.y, 30, position.height);

        Rect xLabelRect = new Rect(position.x - labelSpacing/2f, position.y, labelSpacing, position.height);
        Rect xFieldRect = new Rect(position.x + 1f, position.y, fieldWidth, position.height);

        Rect yLabelRect = new Rect(position.x - labelSpacing / 2f + 1 + 131, position.y, labelSpacing, position.height);
        Rect yFieldRect = new Rect(position.x + 1 + 131, position.y , fieldWidth, position.height);

        Rect zLabelRect = new Rect(position.x - labelSpacing / 2f + 2 + 262, position.y, labelSpacing, position.height);
        Rect zFieldRect = new Rect(position.x + 2 + 262, position.y, fieldWidth, position.height);

        // Ottieni i valori attuali del Vector3
        Vector3 value = property.vector3Value;
        Vector3 lastValue = property.vector3Value;      

        toggle = EditorGUI.Toggle(toggleRect, toggle);

        // Disegna etichette e campi numerici
        value.x = DraggableLabel(xLabelRect, "X", value.x);
        value.x = EditorGUI.FloatField(xFieldRect, value.x);

        value.y = DraggableLabel(yLabelRect, "Y", value.y);
        value.y = EditorGUI.FloatField(yFieldRect, value.y);

        value.z = DraggableLabel(zLabelRect, "Z", value.z);
        value.z = EditorGUI.FloatField(zFieldRect, value.z);

        // Disegna i campi di input con possibilità di trascinare i valori

        if (toggle == true) {
            if (scaleGot) {
                float changeX = (value.x + 0.0001f) / (lastValue.x + 0.0001f);
                float changeY = (value.y + 0.0001f) / (lastValue.y + 0.0001f);
                float changeZ = (value.z + 0.0001f) / (lastValue.z + 0.0001f);

                if (changeX != 1) {
                    value.z = scale.z * value.x;
                    value.y = scale.y * value.z;
                }
                if (changeY != 1) {
                    value.x = scale.x * value.y;
                    value.z = scale.z * value.x;
                }
                if (changeZ != 1) {
                    value.y = scale.y * value.z;
                    value.x = scale.x * value.y;
                }

                value.x = (float)System.Math.Round(value.x, 2);
                value.y = (float)System.Math.Round(value.y, 2);
                value.z = (float)System.Math.Round(value.z, 2);

            }
            else {
                scale.x = (value.x + Mathf.Epsilon) / (value.y + Mathf.Epsilon);
                scale.y = (value.y + Mathf.Epsilon) / (value.z + Mathf.Epsilon);
                scale.z = (value.z + Mathf.Epsilon) / (value.x + Mathf.Epsilon);

                scaleGot = true;
            }
        }
        else {
            scaleGot = false;
        }

        // Applica i nuovi valori al campo
        property.vector3Value = value;
    }
    private float DraggableLabel(Rect position, string label, float value) {
        int id = GUIUtility.GetControlID(FocusType.Passive);
        Event evt = Event.current;

        EditorGUIUtility.AddCursorRect(new Rect(position.x + 15, position.y, 15, position.height), MouseCursor.SlideArrow);

        // Disegna la lettera (X, Y, Z)
        EditorGUI.LabelField(position, label);

        // Rendi la lettera interattiva
        if (evt.type == EventType.MouseDown && position.Contains(evt.mousePosition)) {
            GUIUtility.hotControl = id;
            evt.Use();
        }

        if (GUIUtility.hotControl == id) {
            if (evt.type == EventType.MouseDrag) {
                float tempValue = Mathf.Abs(value);
                float molt = Mathf.Max(0.03f * Mathf.Sqrt(tempValue), 0.03f);
                value = (float)System.Math.Round(value + evt.delta.x * molt, 2); // Incremento basato sul movimento del mouse
                evt.Use();
            }
            else if (evt.type == EventType.MouseUp) {
                GUIUtility.hotControl = 0;
                evt.Use();
            }
        }

        return value;
    }
}
