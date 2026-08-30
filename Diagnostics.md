# Diagnostics

The same identifier is used for the same meaning on every platform, so an identifier can be unused on a platform.

| ID | Severity | Description | How to fix |
|---|---|---|---|
| BTXP0001 | ❌ Error | The property is not declared as a partial property | Declare the property as `public partial T Name { get; set; }` |
| BTXP0002 | ❌ Error | The property is static, and a static property can not be backed by an instance value | Remove `static` from the property, or register the property by hand |
| BTXP0003 | ❌ Error | The property does not have both accessors, or an accessor has its own accessibility modifier such as `private set` | Declare the property as `{ get; set; }` without accessor modifiers |
| BTXP0004 | ❌ Error | The containing type, or one of its outer types, is not partial | Add `partial` to the containing type and to every outer type |
| BTXP0005 | ❌ Error | The containing type has an explicit base type that is not derived from the property host type, so `GetValue` and `SetValue` are not available. A type with no explicit base type is not checked, because the base type can be declared in another partial declaration such as one generated from XAML | Derive the containing type from the property host type |
| BTXP0006 | ❌ Error | The containing type is generic, and a static property field would be created per type argument | Move the property to a non generic type |
| BTXP0007 | ❌ Error | More than one of `DefaultValue`, `DefaultValueExpression` and `DefaultValueMember` is specified | Leave a single default value specification |
| BTXP0008 | ❌ Error | The method specified for a callback does not exist in the containing type or its base types | Specify the method with `nameof`, and define it in the same type or a base type |
| BTXP0009 | ❌ Error | The signature of the specified callback method does not match, or more than one overload is applicable | Match the signature required by the callback |
| BTXP0010 | ❌ Error | The value specified for `DefaultValue` can not be written as a constant in the generated code | Use `DefaultValueExpression` or `DefaultValueMember` |
| BTXP0011 | ❌ Error | The member specified for `DefaultValueMember` is not a static field or property of the property type | Specify a static member whose type matches the property type |
| BTXP0012 | ❌ Error | The method with `[AttachedProperty]` is not a `static partial` getter with a single parameter and a return type | Declare the method as `public static partial T Get<Name>(TTarget obj);` |
| BTXP0013 | ❌ Error | The target type of the `[AttachedProperty]` getter is not derived from the property host type | Use a type derived from the property host type as the parameter type |
