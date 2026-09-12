using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UniVRM10;

namespace yuna0x0.AliceChan.Editor.Tests
{
    // Checks the shipped .vrm imports as a VRM 1.0 avatar on the Unity version running the tests.
    public class AvatarAssetTests
    {
        const string VrmPath = "Packages/com.yuna0x0.alice-chan/Models/AliceChan.vrm";
        const string PrefabPath = "Packages/com.yuna0x0.alice-chan/Models/AliceChan.prefab";

        // A fresh Library can import the model before UniVRM's shaders exist (see
        // ImportOrderCheck). The editor script repairs that on its next update, which in batch
        // mode may come after the tests, so the repair runs here as well.
        [OneTimeSetUp]
        public void RepairImportOrder()
        {
            ImportOrderCheck.Repair();
        }

        static GameObject LoadModel()
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(VrmPath);
            Assert.That(model, Is.Not.Null, VrmPath + " did not import as a GameObject.");
            return model;
        }

        [Test]
        public void ImportsAsVrm10()
        {
            var instance = LoadModel().GetComponent<Vrm10Instance>();
            Assert.That(instance, Is.Not.Null, "No Vrm10Instance on the imported model.");
            Assert.That(instance.Vrm, Is.Not.Null, "Vrm10Instance has no VRM10Object.");
            Assert.That(instance.Vrm.Meta, Is.Not.Null, "The VRM has no meta.");
            Assert.That(instance.Vrm.Meta.Name, Is.EqualTo("Alice Chan (Uniform ver.)"));
        }

        [Test]
        public void IsHumanoid()
        {
            var animator = LoadModel().GetComponent<Animator>();
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.avatar, Is.Not.Null);
            Assert.That(animator.avatar.isHuman, Is.True);
        }

        [Test]
        public void MaterialsUseMToon10()
        {
            var materials = LoadModel().GetComponentsInChildren<Renderer>(true)
                .SelectMany(r => r.sharedMaterials)
                .Distinct()
                .ToList();
            Assert.That(materials, Is.Not.Empty);
            foreach (var material in materials)
            {
                Assert.That(material, Is.Not.Null, "A renderer has a null material slot.");
                Assert.That(material.shader, Is.Not.Null, material.name + " has no shader.");
                Assert.That(material.shader.name, Does.StartWith("VRM10/"),
                    material.name + " uses " + material.shader.name);
            }
        }

        [Test]
        public void HasExpressions()
        {
            var expressions = LoadModel().GetComponent<Vrm10Instance>().Vrm.Expression;
            Assert.That(expressions, Is.Not.Null);
            Assert.That(expressions.Clips.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void PrefabVariantExists()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null, PrefabPath + " is missing.");
            Assert.That(prefab.GetComponent<Vrm10Instance>(), Is.Not.Null);
        }
    }
}
