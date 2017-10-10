require 'json'

APIKEY = ENV['api_key'].nil? ? '' : ENV['api_key']

COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
RESULTS_DIR = "artifacts"
BUILD_VERSION = '1.3.0'

tc_build_number = ENV["APPVEYOR_BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number

CI = ENV["CI"].nil? ? false : true


"Gets the documentation assets ready"
task :prepare_docs do
	sh "dotnet restore docs.csproj"
end

"Launches the documentation project in editable mode"
task :docs => [:prepare_docs] do
	sh "dotnet stdocs run -v #{BUILD_VERSION}"
end

"Exports the documentation to jasperfx.github.io/oakton - requires Git access to that repo though!"
task :publish => [:prepare_docs] do
	if Dir.exists? 'doc-target'
		FileUtils.rm_rf 'doc-target'
	end

	Dir.mkdir 'doc-target'
	sh "git clone https://github.com/jasperfx/jasperfx.github.io.git doc-target"


	sh "dotnet stdocs export doc-target ProjectWebsite --version #{BUILD_VERSION} --project oakton"

	Dir.chdir "doc-target" do
		sh "git add --all"
		sh "git commit -a -m \"Documentation Update for #{BUILD_VERSION}\" --allow-empty"
		sh "git push origin master"
	end




end


def load_project_file(project)
  File.open(project) do |file|
    file_contents = File.read(file, :encoding => 'bom|utf-8')
    JSON.parse(file_contents)
  end
end

module OS
  def OS.windows?
    (/cygwin|mswin|mingw|bccwin|wince|emx/ =~ RUBY_PLATFORM) != nil
  end

  def OS.mac?
   (/darwin/ =~ RUBY_PLATFORM) != nil
  end

  def OS.unix?
    !OS.windows?
  end

  def OS.linux?
    OS.unix? and not OS.mac?
  end
end
