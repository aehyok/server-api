%define _binaries_in_noarch_packages_terminate_build   0
%define user_group dvs
%define installpath /usr/local/sunlight/dvs
%define logpath /var/log/sunlight/dvs-swagger

Name:           dvs-swagger
License:        GPL
Group:          sunlight
Url:            www.sunlight-tech.com
Version:        _VERSION_
Release:        _RELEASE_
Summary:        Sunlight Dvs Cms
Source:         %{name}.%{version}.%{release}.tar.gz
Source1:        %{name}.service
Source2:        %{name}.appsettings.json
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
BuildArch:      noarch
BuildRequires:  systemd
Conflicts:      %{name} <= %{version}

%description

%prep
%setup -c

%install
install -d $RPM_BUILD_ROOT%{installpath}/%{name}
cp -a %{name}.%{version}.%{release}/* $RPM_BUILD_ROOT%{installpath}/%{name}/
install -D -m 0644 %{SOURCE1} %{buildroot}%{_unitdir}/%{name}.service
install -D -m 0644 %{SOURCE2} $RPM_BUILD_ROOT%{installpath}/etc/%{name}.appsettings.json

%post
%service_add_post %{name}.service
mkdir -p %{logpath}
chown -R %{user_group}:%{user_group} %{logpath}

%postun
%service_del_postun %{name}.service

%pre 
%service_add_pre %{name}.service
%{_sbindir}/groupadd -r %{user_group} &>/dev/null ||:
%{_sbindir}/useradd -g %{user_group} -s /bin/false -r -c "user for %{user_group}" %{user_group} &>/dev/null ||:

%preun
%service_del_preun %{name}.service

%clean
[ "$RPM_BUILD_ROOT" != "/" ] && rm -rf $RPM_BUILD_ROOT
rm -rf $RPM_BUILD_DIR/%{name}-%{version}

%files
%defattr(-, dvs, dvs)
%config(noreplace) %{installpath}/etc/%{name}.appsettings.json
%{_unitdir}/%{name}.service
%{installpath}
