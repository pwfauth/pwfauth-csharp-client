# PWF Auth C# Desktop Client

An English Windows Forms sample for the official `PWFAuth` NuGet package. The
application signs in with a license key, keeps the authenticated session alive,
responds to server-side session termination, and displays the complete login
response in a dashboard.

## Requirements

- Windows 10 or later
- Visual Studio with the .NET desktop development workload
- .NET Framework 4.8.1

## Configure

Set the 64-character application secret in
`PWFAuthCSharp/App.config`:

```xml
<add key="PWFAuthAppSecret" value="YOUR_64_CHARACTER_APP_SECRET" />
```

Alternatively, set the `PWFAUTH_APP_SECRET` environment variable. Never commit a
real application secret or license key to the repository.

## Build and run

1. Open `PWFAuthCSharp.slnx` in Visual Studio.
2. Restore NuGet packages when prompted.
3. Select `Debug | Any CPU` and press `F5`.

The sample targets .NET Framework 4.8.1 and uses the `PWFAuth` NuGet package.
