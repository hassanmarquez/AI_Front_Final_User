# Despliegue Local de la Aplicación

## Requisitos Previos

1. **Instalar .NET MAUI**:
   - Asegúrate de tener .NET MAUI instalado. Puedes seguir la guía oficial de instalación [aquí](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-8.0&tabs=visual-studio-code).

2. **Instalar Dependencias de Android**:
   - Ejecuta el siguiente comando para instalar las dependencias de Android:
     ```sh
     dotnet build -t:InstallAndroidDependencies -f:net8.0-android -p:AndroidSdkDirectory="/Users/tu_usuario/Library/Android/sdk" -p:JavaSdkDirectory="/Library/Java/JavaVirtualMachines/microsoft-17.jdk/Contents/Home" -p:AcceptAndroidSDKLicenses=True
     ```

## Clonar el Repositorio

Clona el repositorio en tu máquina local:
```sh
git clone https://github.com/tu_usuario/tu_repositorio.git
cd tu_repositorio
```

## Configurar el Proyecto

1. **Restaurar Paquetes**:
   - Ejecuta el siguiente comando para restaurar los paquetes NuGet necesarios:
     ```sh
     dotnet restore
     ```

2. **Compilar el Proyecto**:
   - Compila el proyecto para asegurarte de que todo está configurado correctamente:
     ```sh
     dotnet build
     ```

## Ejecutar la Aplicación

1. **Configurar el Emulador de Android**:
   - Asegúrate de tener un emulador de Android configurado y ejecutándose. Puedes configurar uno desde Android Studio.

2. **Ejecutar en Android**:
   - Ejecuta el siguiente comando para desplegar la aplicación en un emulador o dispositivo Android:
     ```sh
     dotnet build -t:Run -f:net8.0-android
     ```

3. **Ejecutar en iOS**:
   - Para ejecutar en un dispositivo iOS, asegúrate de tener Xcode instalado y configurado. Luego, ejecuta:
     ```sh
     dotnet build -t:Run -f:net8.0-ios
     ```

4. **Ejecutar en MacCatalyst**:
   - Para ejecutar en MacCatalyst, ejecuta:
     ```sh
     dotnet build -t:Run -f:net8.0-maccatalyst
     ```

5. **Ejecutar en Windows**:
   - Para ejecutar en Windows, asegúrate de tener las herramientas de desarrollo de Windows configuradas y ejecuta:
     ```sh
     dotnet build -t:Run -f:net8.0-windows10.0.19041.0
     ```

## Configuración Adicional

1. **Permisos de Bluetooth y Ubicación**:
   - Asegúrate de que la aplicación tiene los permisos necesarios para Bluetooth y ubicación. Puedes verificar esto en los archivos de manifiesto correspondientes, como `AndroidManifest.xml` y `Info.plist`.

2. **Configuración de Notificaciones Locales**:
   - La aplicación utiliza notificaciones locales. Asegúrate de que los permisos necesarios están configurados y que las notificaciones están habilitadas en tu dispositivo.

## Solución de Problemas

- Si encuentras problemas durante la compilación o ejecución, verifica los mensajes de error en la consola y asegúrate de que todas las dependencias están correctamente instaladas.
- Consulta la documentación oficial de .NET MAUI para obtener más detalles sobre la configuración y solución de problemas.

## Enlaces Útiles

- [Guía de Instalación de .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-8.0&tabs=visual-studio-code)
- [Documentación de .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/overview)

Con estos pasos, deberías poder desplegar y ejecutar la aplicación localmente en tu máquina.
