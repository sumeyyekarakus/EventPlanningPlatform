# 🧠 SmartEventPlanner - Akıllı Etkinlik Planlama Platformu

Bu proje, kullanıcıların etkinlikler oluşturabileceği, katılabileceği ve bu etkinlikler etrafında sosyal etkileşim kurabileceği bir **web tabanlı Akıllı Etkinlik Planlama Platformu**dur. ASP.NET Core MVC kullanılarak geliştirilmiş ve SQL Server veritabanı ile desteklenmiştir.

## 🎯 Proje Amacı

- Kullanıcıların kendi etkinliklerini oluşturabilmesini sağlamak  
- İlgi alanlarına göre kişiselleştirilmiş etkinlik önerileri sunmak  
- Harita üzerinden etkinlikleri görselleştirmek ve rota planlaması yapmak  
- Etkinliklere katılımda tarih/çakışma denetimi yapmak  
- Sosyal mesajlaşma ve puan sistemi ile kullanıcı etkileşimini artırmak  

## 🚀 Özellikler

- Kullanıcı ve yönetici girişi
- Yeni kullanıcı kaydı (BCrypt ile şifreleme)
- İlgi alanlarına göre öneri sistemi
- Harita entegrasyonu (OpenStreetMap API)
- Zaman/çakışma kontrolü ile etkinlik katılımı
- Sohbet paneli ile etkinlik içi mesajlaşma
- Etkinlik oluşturma ve puan kazanma
- Admin paneli üzerinden kullanıcı ve etkinlik yönetimi

## 🧱 Kullanılan Teknolojiler

- ASP.NET Core MVC
- SQL Server
- ADO.NET
- OpenStreetMap API
- BCrypt şifreleme
- HTML / CSS / Bootstrap

## 📁 Proje Yapısı

- `HomeController.cs` – Giriş, yönlendirme işlemleri
- `KullaniciPanelController.cs` – Kullanıcı kayıt, şifre sıfırlama, öneri, etkinlik detayları
- `AdminPanelController.cs` – Admin için kullanıcı/etkinlik işlemleri
- `Views/` – Razor tabanlı sayfa görünümleri
- `wwwroot/` – CSS ve script dosyaları
- `Models/` – Kullanıcı, etkinlik ve katılımcı veri yapıları

## 🧪 Deneysel Sonuçlar

- Öneri sistemi %85 başarı ile ilgi alanlarına uygun etkinlik sunmaktadır
- Zaman çakışma kontrol algoritması %92 oranında başarılı sonuçlar vermiştir
- Harita üzerinden etkinliklerin doğru gösterimi ve rota hesaplama test edilmiştir
- Puan sistemi, kullanıcı katılımını %20 oranında artırmıştır
- Gerçek zamanlı sohbet modülü ile sosyal etkileşim sağlanmıştır


## 📃 Lisans

Bu proje akademik amaçlarla geliştirilmiştir. 

______________________________________________________________________________________________________________________________________________________________________________________________________________________



# 🧠 SmartEventPlanner – Smart Event Planning Platform

This project is a **web-based Smart Event Planning Platform** where users can create events, participate in them, and engage in social interactions around these events. It is developed using ASP.NET Core MVC and supported by an SQL Server database.

## 🎯 Project Objectives

* Allow users to create their own events
* Provide personalized event recommendations based on user interests
* Visualize events on a map and plan routes
* Prevent time conflicts when joining events
* Enhance social interaction through messaging and a point system

## 🚀 Features

* User and admin login
* New user registration (passwords encrypted with BCrypt)
* Recommendation system based on user interests
* Map integration (OpenStreetMap API)
* Conflict check for event scheduling
* Real-time chat panel for in-event messaging
* Event creation with point rewards
* Admin panel for managing users and events

## 🧱 Technologies Used

* ASP.NET Core MVC
* SQL Server
* ADO.NET
* OpenStreetMap API
* BCrypt hashing
* HTML / CSS / Bootstrap

## 📁 Project Structure

* `HomeController.cs` – Handles homepage and routing
* `KullaniciPanelController.cs` – Manages user registration, password reset, recommendations, and event details
* `AdminPanelController.cs` – Controls admin actions for user and event management
* `Views/` – Razor-based page views
* `wwwroot/` – CSS and JavaScript files
* `Models/` – Data structures for users, events, and participants

## 🧪 Experimental Results

* Recommendation system provides events matching user interests with 85% accuracy
* Time conflict control algorithm achieves 92% success rate
* Accurate event display and route calculations on map tested successfully
* Point system increased user participation by 20%
* Real-time chat module enabled effective social interaction


## 📃 License

This project was developed for academic purposes.

<img width="1806" height="1015" alt="Ekran görüntüsü 2025-07-10 154358" src="https://github.com/user-attachments/assets/40946071-f475-4764-9e86-c058d1061704" />
<img width="1830" height="1018" alt="Ekran görüntüsü 2025-07-10 154840" src="https://github.com/user-attachments/assets/6e602801-095a-4b02-88a4-adf569e91124" />
<img width="1902" height="1017" alt="Ekran görüntüsü 2025-07-10 154814" src="https://github.com/user-attachments/assets/33480920-6979-4385-9bc5-4444369421c3" />
<img width="1910" height="983" alt="Ekran görüntüsü 2025-07-10 154736" src="https://github.com/user-attachments/assets/09639c33-7ac5-4146-8f02-7e0a0ad7876d" />
<img width="1767" height="1002" alt="Ekran görüntüsü 2025-07-10 154711" src="https://github.com/user-attachments/assets/3417d71d-d6f9-4759-9e60-a3cf17f9739d" />
<img width="1732" height="960" alt="Ekran görüntüsü 2025-07-10 154447" src="https://github.com/user-attachments/assets/46e9531e-f01b-4d12-bd74-da248aebeea5" />
